namespace RentalFlow.Application.Requests.Property;

public sealed record UpdatePropertyRequest
{
    [Description("The address of the property.")]
    public Address? Address { get; init; }

    [Description("The monthly rent price of the property.")]
    public decimal? RentPrice { get; init; }

    [Description("The number of bedrooms in the property.")]
    public int? Bedrooms { get; init; }

    [Description("Indicates if the property is available for rent.")]
    public bool? IsAvailable { get; init; }

    [Description("Indicates if the property is active.")]
    public bool? IsActive { get; init; }
}