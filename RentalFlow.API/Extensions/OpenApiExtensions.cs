namespace RentalFlow.API.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddRentalFlowOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new();

                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes.Add(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Name = "Authorization",
                        In = ParameterLocation.Header
                    });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseRentalFlowOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options.DarkMode = true;
                options.HideDarkModeToggle = true;
                options.HideClientButton = true;
                options.HideModels = true;
                options.HideSearch = true;

                options.WithTitle("RentalFlow API");
                options.WithTheme(ScalarTheme.Default);
                options.AddPreferredSecuritySchemes("Bearer");
            });
        }

        return app;
    }
}