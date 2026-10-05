namespace RentalFlow.API.Extensions;

public static class OpenApiExtension
{
    public static IServiceCollection AddRentalFlowOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<DescriptionTransformer>();
            options.AddDocumentTransformer<SecurityTransformer>();
            options.AddDocumentTransformer<TagDescriptionTransformer>();
        });

        return services;
    }

    public static WebApplication UseRentalFlowOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
            .WithTitle("RentalFlow API")
            .WithTheme(ScalarTheme.Default)
            .AddPreferredSecuritySchemes("Bearer");
        });

        return app;
    }
}