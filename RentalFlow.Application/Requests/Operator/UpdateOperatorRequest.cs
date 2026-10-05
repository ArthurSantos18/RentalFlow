namespace RentalFlow.Application.Requests.Operator;

public sealed record UpdateOperatorRequest
{
    [Description("The name of the operator.")]
    public string? Name { get; init; }

    [Description("The email address of the operator.")]
    public string? Email { get; init; }

    [Description("The role of the operator.")]
    public OperatorRole? Role { get; init; }

    [Description("Indicates if the operator is active.")]
    public bool? IsActive { get; init; }
}