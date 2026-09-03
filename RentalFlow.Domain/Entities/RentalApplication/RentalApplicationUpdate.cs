using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.RentalApplication;

public record RentalApplicationUpdate(decimal? FinancedAmount,
    decimal? TotalAmount,
    int? Installments,
    RentalStatus? Status,
    DateTime? ContractDate,
    bool? IsActive);
