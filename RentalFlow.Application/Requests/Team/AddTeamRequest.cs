namespace RentalFlow.Application.Requests.Team;

public record AddTeamRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
