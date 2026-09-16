namespace RentalFlow.Application.Responses;

public sealed record GetTeamResponse : BaseResponse
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
