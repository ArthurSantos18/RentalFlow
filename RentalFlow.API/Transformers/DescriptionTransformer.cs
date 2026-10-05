namespace RentalFlow.API.Transformers;

[ExcludeFromCodeCoverage(Justification = "Contains no logic, only metadata for API documentation.")]
public sealed class DescriptionTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var overviewPath = Path.Combine(AppContext.BaseDirectory, "Metadata", "OVERVIEW.md");

        if (!File.Exists(overviewPath))
        {
            return;
        }

        var description = await File.ReadAllTextAsync(overviewPath, cancellationToken);

        document.Info ??= new OpenApiInfo();
        document.Info.Title = "RentalFlow API";
        document.Info.Description = description;
        document.Info.Version = "v1";
        document.Info.Contact = new OpenApiContact
        {
            Name = "Arthur Santos",
            Url = new Uri("https://github.com/ArthurSantos18")
        };
    }
}