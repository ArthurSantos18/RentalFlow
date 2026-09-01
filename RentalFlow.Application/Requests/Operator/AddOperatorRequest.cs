using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Requests.Operator;

public record AddOperatorRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public OperatorRole Role { get; init; }
}
