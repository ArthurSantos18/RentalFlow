namespace RentalFlow.Application.Requests.Property;

public sealed record AddPropertyRequest
{
    public Address Address { get; init; } = Address.Empty;
    public decimal RentPrice {  get; init; }
    public int Bedrooms { get; init; }
    public bool IsAvailable { get; init; }
}
