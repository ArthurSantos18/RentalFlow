namespace RentalFlow.Application.Requests.Team;

public sealed record UpdateTeamRequest
{
    [Description("The new name for the team.")]
    public string? Name { get; init; }

    [Description("The new description for the team.")]
    public string? Description { get; init; }

    [Description("The new active status for the team.")]
    public bool? IsActive { get; init; }
}