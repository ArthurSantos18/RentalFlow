namespace RentalFlow.Application.Requests.Operator;

public sealed record AddOperatorRequest
{
    [Description("The name of the operator.")]
    public string Name { get; init; } = string.Empty;

    [Description("The email address of the operator.")]
    public string Email { get; init; } = string.Empty;

    [Description("The role of the operator.")]
    public OperatorRole Role { get; init; }

    [Description("The ID of the team to which the operator belongs.")]
    public Guid TeamId { get; init; }
}