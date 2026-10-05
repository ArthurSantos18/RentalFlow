namespace RentalFlow.Application.Requests.Applicant;

public sealed record GetApplicantRequest
{
    public GetApplicantRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of applicant IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of full names to filter by.")]
    public IEnumerable<string>? FullNames { get; set; }

    [Description("The list of CPFs to filter by.")]
    public IEnumerable<string>? Cpfs { get; set; }

    [Description("The list of emails to filter by.")]
    public IEnumerable<string>? Emails { get; set; }

    [Description("The list of phones to filter by.")]
    public IEnumerable<string>? Phones { get; set; }

    [Description("The minimum monthly income to filter by.")]
    public decimal? MinMonthlyIncome { get; set; }

    [Description("The maximum monthly income to filter by.")]
    public decimal? MaxMonthlyIncome { get; set; }

    [Description("Indicates if the applicant is active.")]
    public bool? IsActive { get; set; }

    [Description("Indicates if the applicant has applications.")]
    public bool? HasApplications { get; set; }

    [Description("The list of application IDs to filter by.")]
    public IEnumerable<Guid>? ApplicationIds { get; set; }
};