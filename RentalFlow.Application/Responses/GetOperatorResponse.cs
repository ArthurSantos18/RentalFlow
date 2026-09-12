using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Responses;

public record GetOperatorResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public OperatorRole Role { get; init; }
    public bool IsActive { get; init; }
    public Guid TeamId { get; init; }
    public string TeamName { get; init; } = string.Empty;

}
