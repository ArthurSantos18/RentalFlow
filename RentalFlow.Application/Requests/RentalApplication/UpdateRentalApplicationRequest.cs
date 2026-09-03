using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Requests.RentalApplication;

public record UpdateRentalApplicationRequest
{
    public int? Installments { get; init; }
    public decimal? FinancedAmount { get; init; }
    public decimal? TotalAmount { get; init; }
    public RentalStatus? Status { get; init; }
    public DateTime? ContractDate { get; init; }
    public bool? IsActive { get; init; }
}
