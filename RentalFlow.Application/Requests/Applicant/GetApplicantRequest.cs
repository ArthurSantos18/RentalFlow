namespace RentalFlow.Application.Requests.Applicant;

public sealed record GetApplicantRequest
{
    public GetApplicantRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs de inquilinos para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de nomes completos para filtrar.")]
    public IEnumerable<string>? FullNames { get; set; }

    [Description("A lista de CPFs para filtrar.")]
    public IEnumerable<string>? Cpfs { get; set; }

    [Description("A lista de emails para filtrar.")]
    public IEnumerable<string>? Emails { get; set; }

    [Description("A lista de telefones para filtrar.")]
    public IEnumerable<string>? Phones { get; set; }

    [Description("A renda mensal mínima para filtrar.")]
    public decimal? MinMonthlyIncome { get; set; }

    [Description("A renda mensal máxima para filtrar.")]
    public decimal? MaxMonthlyIncome { get; set; }

    [Description("Indica se o inquilino está ativo.")]
    public bool? IsActive { get; set; }

    [Description("Indica se o inquilino possui aplicações.")]
    public bool? HasApplications { get; set; }

    [Description("A lista de IDs de aplicações para filtrar.")]
    public IEnumerable<Guid>? ApplicationIds { get; set; }
};