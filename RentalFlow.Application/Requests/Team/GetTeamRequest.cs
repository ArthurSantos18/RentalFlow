namespace RentalFlow.Application.Requests.Team;

public sealed record GetTeamRequest
{
    public GetTeamRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of team IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of team names to filter by.")]
    public IEnumerable<string>? Names { get; set; }

    [Description("The list of team descriptions to filter by.")]
    public IEnumerable<string>? Description { get; set; }

    [Description("Indicates if the team is active.")]
    public bool? IsActive { get; set; }
}