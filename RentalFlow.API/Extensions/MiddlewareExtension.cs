namespace RentalFlow.API.Extensions;

public static class MiddlewareExtension
{
    public static void UseRequestIdMiddleware(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
    }

    public static void UseRequestLogContext(this WebApplication app)
    {
        app.UseMiddleware<LogContextMiddleware>();
    }
}