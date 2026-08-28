using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Application.Requests.Property;

public record AddPropertyRequest
{
    public Address Address { get; init; } = Address.Empty;
    public decimal RentPrice {  get; init; }
    public int Bedrooms { get; init; }
    public bool IsAvailable { get; init; }
}
