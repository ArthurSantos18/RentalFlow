namespace RentalFlow.Application.Requests.RentalApplication;

public record UpdateRentalApplicationRequest
{
    public int? Installments { get; init; }
    public decimal? FinancedAmount { get; init; }
    public decimal? TotalAmount { get; init; }
    public DateTime? ContractDate { get; init; }
    public Guid? ApplicantId { get; init; }
    public Guid? OperatorId { get; init; }
    public Guid? PropertyId { get; init; }
}
