namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class RateLimitingExtension
{
    public static IServiceCollection AddRentalFlowRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
         {
             options.OnRejected = async (context, cancellationToken) =>
             {
                 var logger = context.HttpContext.RequestServices
                     .GetRequiredService<ILoggerFactory>()
                     .CreateLogger("RentalFlow.RateLimiting");

                 var path = context.HttpContext.Request.Path.ToString();
                 var method = context.HttpContext.Request.Method;
                 var retryAfter = GetRetryAfter(context);

                 logger.LogWarning(
                     "Rate limit exceeded on {Method} {Path}. RetryAfter: {RetryAfter}s",
                     method,
                     path,
                     retryAfter);

                 var errorResponse = new ErrorResponse
                 {
                     Code = StatusCodes.Status429TooManyRequests,
                     Message = "Too many requests. Please try again later."
                 };

                 if (retryAfter != null)
                 {
                     context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();
                 }

                 context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                 context.HttpContext.Response.ContentType = "application/problem+json";

                 await context.HttpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
             };

             options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
             {
                 var partitionKey = GetPartitionKey(httpContext);

                 return RateLimitPartition.GetSlidingWindowLimiter(
                     partitionKey, _ => new SlidingWindowRateLimiterOptions
                     {
                         PermitLimit = 200,
                         Window = TimeSpan.FromMinutes(1),
                         SegmentsPerWindow = 6,
                         QueueLimit = 0
                     });
             });

             options.AddPolicy(RateLimitingPolicies.LoginPolicy, httpContext =>
             {
                 var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                 return RateLimitPartition.GetFixedWindowLimiter(
                     ip,
                     _ => new FixedWindowRateLimiterOptions
                     {
                         PermitLimit = 5,
                         Window = TimeSpan.FromMinutes(1),
                         QueueLimit = 0
                     });
             });

             options.AddPolicy(RateLimitingPolicies.RefreshPolicy, httpContext =>
             {
                 var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                 return RateLimitPartition.GetSlidingWindowLimiter(
                     ip,
                     _ => new SlidingWindowRateLimiterOptions
                     {
                         PermitLimit = 10,
                         Window = TimeSpan.FromMinutes(1),
                         SegmentsPerWindow = 6,
                         QueueLimit = 0
                     });
             });

             options.AddPolicy(RateLimitingPolicies.AuthenticatedPolicy, httpContext =>
             {
                 var partitionKey = GetPartitionKey(httpContext);

                 return RateLimitPartition.GetSlidingWindowLimiter(
                     partitionKey,
                     _ => new SlidingWindowRateLimiterOptions
                     {
                         PermitLimit = 100,
                         Window = TimeSpan.FromMinutes(1),
                         SegmentsPerWindow = 6,
                         QueueLimit = 0
                     });
             });
         });

        return services;
    }

    public static WebApplication UseRentalFlowRateLimiting(this WebApplication app)
    {
        app.UseRateLimiter();
        return app;
    }

    private static int? GetRetryAfter(OnRejectedContext context)
    {
        if (!context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            return null;
        }

        return Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
    }

    private static string GetPartitionKey(HttpContext httpContext)
    {
        var userId = httpContext.User?.FindFirst("sub")?.Value ?? httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return $"ip:{ip}";
    }
}
