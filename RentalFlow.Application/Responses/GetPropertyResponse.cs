namespace RentalFlow.Application.Responses;

public sealed record GetPropertyResponse : BaseResponse
{
    public Address Address { get; init; } = Address.Empty;
    public decimal RentPrice { get; init; }
    public int Bedrooms { get; init; }
    public bool IsAvailable { get; init; }
}
