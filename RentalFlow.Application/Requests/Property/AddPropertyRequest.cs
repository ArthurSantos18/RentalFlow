namespace RentalFlow.Application.Requests.Property;

public sealed record AddPropertyRequest
{
    [Description("The address of the property.")]
    public Address Address { get; init; } = Address.Empty;

    [Description("The monthly rent price of the property.")]
    public decimal RentPrice { get; init; }

    [Description("The number of bedrooms in the property.")]
    public int Bedrooms { get; init; }

    [Description("Indicates if the property is available for rent.")]
    public bool IsAvailable { get; init; }
}