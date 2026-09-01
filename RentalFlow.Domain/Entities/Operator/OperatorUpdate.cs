using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.Operator;

public sealed record OperatorUpdate(
    string? Name,
    string? Email,
    OperatorRole? Role);