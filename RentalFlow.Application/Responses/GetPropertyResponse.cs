using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Application.Responses;

public record GetPropertyResponse
{
    public Guid Id { get; init; }
    public Address Address { get; init; } = Address.Empty;
    public decimal RentPrice { get; init; }
    public int Bedrooms { get; init; }
    public bool IsAvailable { get; init; }
    public bool IsActive { get; init; }

}
