namespace RentalFlow.Application.Responses;

public record AddOperatorResponse
{
    [Description("The unique identifier of the newly added operator.")]
    public Guid Id { get; init; }

    [Description("The name of the newly added operator.")]
    public string Name { get; init; } = string.Empty;

    [Description("The email of the newly added operator.")]
    public string Email { get; init; } = string.Empty;

    [Description("The role of the newly added operator.")]
    public string Role { get; init; } = string.Empty;

    [Description("The temporary password for the newly added operator.")]
    public string TemporaryPassword { get; init; } = string.Empty;

    [Description("A message associated with the response.")]
    public string Message { get; init; } = string.Empty;
}