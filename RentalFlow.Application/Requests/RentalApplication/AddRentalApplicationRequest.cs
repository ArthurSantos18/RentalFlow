namespace RentalFlow.Application.Requests.RentalApplication;

public record AddRentalApplicationRequest
{
    public Guid ApplicantId { get; init; }
    public Guid PropertyId { get; init; }
    public Guid OperatorId { get; init; }
    public decimal FinancedAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public int Installments { get; init; }
    public DateTime? ContractDate { get; init; }
}