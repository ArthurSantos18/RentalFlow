namespace RentalFlow.Application.Requests.Operator;

public sealed record UpdateOperatorRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public OperatorRole? Role { get; init; }
    public Guid? TeamId { get; init; }
    public bool? IsActive { get; init; }
}
