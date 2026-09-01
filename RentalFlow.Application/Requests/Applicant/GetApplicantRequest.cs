namespace RentalFlow.Application.Requests.Applicant;

public record GetApplicantRequest
{
    public GetApplicantRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? FullNames { get; set; }
    public IEnumerable<string>? Cpfs { get; set; }
    public IEnumerable<string>? Emails { get; set; }
    public IEnumerable<string>? Phones { get; set; }
    public decimal? MinMonthlyIncome { get; set; }
    public decimal? MaxMonthlyIncome { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasApplications { get; set; }
    public IEnumerable<Guid>? ApplicationIds { get; set; }
};
