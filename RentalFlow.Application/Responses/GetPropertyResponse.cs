namespace RentalFlow.Application.Responses;

public sealed record GetPropertyResponse : BaseResponse
{
    public Address Address { get; init; } = Address.Empty;

    [Description("The price of the property for rent.")]
    public decimal RentPrice { get; init; }

    [Description("The number of bedrooms in the property.")]
    public int Bedrooms { get; init; }

    [Description("Indicates whether the property is available for rent.")]
    public bool IsAvailable { get; init; }
}