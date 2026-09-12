namespace RentalFlow.Application.Requests.Team;

public record GetTeamRequest
{
    public GetTeamRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Description { get; set; }
    public bool? IsActive { get; set; }
}
