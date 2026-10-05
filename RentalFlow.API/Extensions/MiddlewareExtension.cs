namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class MiddlewareExtensions
{
    public static WebApplication UseCorrelationIdMiddleware(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    public static WebApplication UseRequestLogContext(this WebApplication app)
    {
        app.UseMiddleware<LogContextMiddleware>();
        return app;
    }
}