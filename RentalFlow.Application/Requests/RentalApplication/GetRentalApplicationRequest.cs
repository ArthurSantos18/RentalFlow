namespace RentalFlow.Application.Requests.RentalApplication;

public sealed class GetRentalApplicationRequest
{
    public GetRentalApplicationRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of rental application IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of proposal numbers to filter by.")]
    public IEnumerable<string>? ProposalNumbers { get; set; }

    [Description("The list of applicant IDs to filter by.")]
    public IEnumerable<Guid>? ApplicantIds { get; set; }

    [Description("The list of property IDs to filter by.")]
    public IEnumerable<Guid>? PropertyIds { get; set; }

    [Description("The list of operator IDs to filter by.")]
    public IEnumerable<Guid>? OperatorIds { get; set; }

    [Description("The list of team IDs to filter by.")]
    public IEnumerable<Guid>? TeamIds { get; set; }

    [Description("The status of the rental application to filter by.")]
    public RentalStatus Status { get; set; }

    [Description("The minimum financed amount to filter by.")]
    public decimal? MinFinancedAmount { get; set; }

    [Description("The maximum financed amount to filter by.")]
    public decimal? MaxFinancedAmount { get; set; }

    [Description("The minimum total amount to filter by.")]
    public decimal? MinTotalAmount { get; set; }

    [Description("The maximum total amount to filter by.")]
    public decimal? MaxTotalAmount { get; set; }

    [Description("The minimum creation date to filter by.")]
    public DateTime? MinCreatedAt { get; set; }

    [Description("The maximum creation date to filter by.")]
    public DateTime? MaxCreatedAt { get; set; }

    [Description("The minimum contract date to filter by.")]
    public DateTime? MinContractDate { get; set; }

    [Description("The maximum contract date to filter by.")]
    public DateTime? MaxContractDate { get; set; }

    [Description("Indicates if the rental application is active.")]
    public bool? IsActive { get; set; }
}