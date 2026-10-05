namespace RentalFlow.Application.Responses;

public sealed record GetOperatorResponse : BaseResponse
{
    [Description("The name of the operator.")]
    public string Name { get; init; } = string.Empty;

    [Description("The role of the operator.")]
    public OperatorRole Role { get; init; }

    [Description("The unique identifier of the team the operator belongs to.")]
    public Guid TeamId { get; init; }

    [Description("The name of the team the operator belongs to.")]
    public string TeamName { get; init; } = string.Empty;

    [Description("The email address of the operator.")]
    public string Email { get; init; } = string.Empty;

    [Description("Indicates whether the operator must change their password.")]
    public bool MustChangePassword { get; init; }
}