namespace RentalFlow.Application.Responses;

public sealed record GetOperatorByIdResponse : BaseResponse
{
    public string Name { get; init; } = string.Empty;
    public OperatorRole Role { get; init; }
    public Guid TeamId { get; init; }
    public string TeamName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool MustChangePassword { get; init; }
    public Guid UserId { get; init; }
    public int ApplicationsCount { get; init; }
}