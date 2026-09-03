using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Responses;

public record GetRentalApplicationResponse
{
    public Guid Id { get; init; }
    public string ProposalNumber { get; init; } = string.Empty;
    public decimal FinancedAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public int Installments { get; init; }
    public RentalStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ContractDate { get; init; }
    public bool IsActive { get; init; }
    public Guid ApplicantId { get; init; }
    public string ApplicantName { get; init; } = string.Empty;
    public string ApplicantCpf { get; init; } = string.Empty;
    public Guid PropertyId { get; init; }
    public string PropertyAddress { get; init; } = string.Empty;
    public decimal PropertyRentPrice { get; init; }
    public Guid OperatorId { get; init; }
    public string OperatorName { get; init; } = string.Empty;
}
