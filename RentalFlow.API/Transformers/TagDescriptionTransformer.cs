namespace RentalFlow.API.Transformers;

[ExcludeFromCodeCoverage(Justification = "Contains no logic, only metadata for API documentation.")]
public sealed class TagDescriptionTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var tagsPath = Path.Combine(AppContext.BaseDirectory, "Metadata", "TAGS.md");

        if (!File.Exists(tagsPath))
        {
            return;
        }

        var content = await File.ReadAllTextAsync(tagsPath, cancellationToken);
        var descriptions = ParseTags(content);

        document.Tags ??= new HashSet<OpenApiTag>();

        foreach (var (name, description) in descriptions)
        {
            var existingTag = document.Tags.FirstOrDefault(t => t.Name == name);

            if (existingTag is not null)
            {
                existingTag.Description = description;
            }
            else
            {
                document.Tags.Add(new OpenApiTag
                {
                    Name = name,
                    Description = description
                });
            }
        }
    }

    private static Dictionary<string, string> ParseTags(string content)
    {
        var result = new Dictionary<string, string>();
        var lines = content.Split('\n');

        string? currentTag = null;
        var buffer = new StringBuilder();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();

            if (line.StartsWith("## "))
            {
                if (currentTag is not null && buffer.Length > 0)
                {
                    result[currentTag] = buffer.ToString().Trim();
                }

                currentTag = line[3..].Trim();
                buffer.Clear();
                continue;
            }

            if (line.StartsWith("# ") || line.Trim() == "---")
            {
                continue;
            }

            if (currentTag is not null)
            {
                buffer.AppendLine(line);
            }
        }

        if (currentTag is not null && buffer.Length > 0)
        {
            result[currentTag] = buffer.ToString().Trim();
        }

        return result;
    }
}
