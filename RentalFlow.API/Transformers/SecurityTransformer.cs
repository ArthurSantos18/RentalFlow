namespace RentalFlow.API.Transformers;

[ExcludeFromCodeCoverage(Justification = "Contains no logic, only metadata for API documentation.")]
public sealed class SecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Informe o token de acesso JWT. Exemplo: `eyJhbGciOiJIUzI1NiIs...`"
        };

        return Task.CompletedTask;
    }
}