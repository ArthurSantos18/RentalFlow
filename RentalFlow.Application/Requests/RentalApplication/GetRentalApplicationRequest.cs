using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Requests.RentalApplication;

public record GetRentalApplicationRequest
{
    public GetRentalApplicationRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? ProposalNumbers { get; set; }
    public IEnumerable<Guid>? ApplicantIds { get; set; }
    public IEnumerable<Guid>? PropertyIds { get; set; }
    public IEnumerable<Guid>? OperatorIds { get; set; }
    public RentalStatus Status { get; set; }
    public decimal? MinFinancedAmount { get; set; }
    public decimal? MaxFinancedAmount { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
    public DateTime? MinCreatedAt { get; set; }
    public DateTime? MaxCreatedAt { get; set; }
    public DateTime? MinContractDate { get; set; }
    public DateTime? MaxContractDate { get; set; }
    public bool? IsActive { get; set; }
}
