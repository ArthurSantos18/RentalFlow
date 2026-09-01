using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Requests.Operator;

public record UpdateOperatorRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public OperatorRole? Role { get; init; }
}
