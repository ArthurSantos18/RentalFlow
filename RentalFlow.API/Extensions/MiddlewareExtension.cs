namespace RentalFlow.API.Extensions;

public static class MiddlewareExtension
{
    public static void UseMiddleware(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<LogContextMiddleware>();
    }
}