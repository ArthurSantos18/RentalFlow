namespace RentalFlow.API.Transformers;

[ExcludeFromCodeCoverage(Justification = "Contains no logic, only metadata for API documentation.")]
public sealed class TagDescriptionTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Metadata", "TAGS.md");

        if (!File.Exists(path))
        {
            return;
        }

        var content = await File.ReadAllTextAsync(path, cancellationToken);
        var blocks = Markdown.Parse(content);

        var headings = blocks.OfType<HeadingBlock>().Where(heading => heading.Level == 2);
        var separators = blocks.OfType<ThematicBreakBlock>();

        document.Tags ??= new HashSet<OpenApiTag>();

        foreach (var (heading, separator) in headings.Zip(separators))
        {
            var name = heading.Inline?.FirstChild?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var description = content[(heading.Span.End + 1)..separator.Span.Start].Trim();

            var existing = document.Tags.FirstOrDefault(tag => string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
            {
                existing.Description = description;
                continue;
            }

            document.Tags.Add(new OpenApiTag
            {
                Name = name,
                Description = description
            });
        }
    }
}
