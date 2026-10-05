namespace RentalFlow.Application.Requests.Team;

public sealed record AddTeamRequest
{
    [Description("The name of the team.")]
    public string Name { get; init; } = string.Empty;

    [Description("The description of the team.")]
    public string Description { get; init; } = string.Empty;
}