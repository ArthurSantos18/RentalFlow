namespace RentalFlow.Application.Requests.RentalApplication;

public sealed class GetRentalApplicationRequest
{
    public GetRentalApplicationRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs de solicitações de aluguel para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de números de propostas para filtrar.")]
    public IEnumerable<string>? ProposalNumbers { get; set; }

    [Description("A lista de IDs de candidatos para filtrar.")]
    public IEnumerable<Guid>? ApplicantIds { get; set; }

    [Description("A lista de IDs de propriedades para filtrar.")]
    public IEnumerable<Guid>? PropertyIds { get; set; }

    [Description("A lista de IDs de operadores para filtrar.")]
    public IEnumerable<Guid>? OperatorIds { get; set; }

    [Description("A lista de IDs de equipes para filtrar.")]
    public IEnumerable<Guid>? TeamIds { get; set; }

    [Description("O status da solicitação de aluguel para filtrar.")]
    public RentalStatus Status { get; set; }

    [Description("O valor mínimo financiado para filtrar.")]
    public decimal? MinFinancedAmount { get; set; }

    [Description("O valor máximo financiado para filtrar.")]
    public decimal? MaxFinancedAmount { get; set; }

    [Description("O valor mínimo total para filtrar.")]
    public decimal? MinTotalAmount { get; set; }

    [Description("O valor máximo total para filtrar.")]
    public decimal? MaxTotalAmount { get; set; }

    [Description("A data mínima de criação para filtrar.")]
    public DateTime? MinCreatedAt { get; set; }

    [Description("A data máxima de criação para filtrar.")]
    public DateTime? MaxCreatedAt { get; set; }

    [Description("A data mínima do contrato para filtrar.")]
    public DateTime? MinContractDate { get; set; }

    [Description("A data máxima do contrato para filtrar.")]
    public DateTime? MaxContractDate { get; set; }

    [Description("Indica se a solicitação de aluguel está ativa.")]
    public bool? IsActive { get; set; }
}