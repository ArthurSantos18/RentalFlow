namespace RentalFlow.API.Middlewares;

public sealed class LogContextMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? context.TraceIdentifier;

        context.Response.Headers["X-Correlation-Id"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            if (currentUserService.IsAuthenticated)
            {
                using (LogContext.PushProperty("UserId", currentUserService.UserId))
                using (LogContext.PushProperty("Role", currentUserService.Role))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}