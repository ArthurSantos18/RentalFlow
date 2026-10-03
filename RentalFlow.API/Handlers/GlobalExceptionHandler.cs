namespace RentalFlow.API.Handlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> _logger,
    IHostEnvironment _environment
    ) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, TraceId: {TraceId}",
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.TraceIdentifier);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Instance = httpContext.Request.Path,
            Detail = _environment.IsDevelopment() ? exception.Message : "An internal server error occurred. Please contact support."
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}