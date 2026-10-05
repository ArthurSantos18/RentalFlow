namespace RentalFlow.Application.Requests.Team;

public sealed record GetTeamRequest
{
    public GetTeamRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs das equipes para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de nomes das equipes para filtrar.")]
    public IEnumerable<string>? Names { get; set; }

    [Description("A lista de descrições das equipes para filtrar.")]
    public IEnumerable<string>? Description { get; set; }

    [Description("Indica se a equipe está ativa.")]
    public bool? IsActive { get; set; }
}