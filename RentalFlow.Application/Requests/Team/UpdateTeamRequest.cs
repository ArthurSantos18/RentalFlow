namespace RentalFlow.Application.Requests.Team;

public record UpdateTeamRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
}
