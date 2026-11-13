namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class ForwardedHeadersExtension
{
    public static WebApplication UseRentalFlowForwardedHeaders(this WebApplication app)
    {
        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };

        app.UseForwardedHeaders(options);

        return app;
    }
}