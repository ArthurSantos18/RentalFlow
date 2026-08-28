using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Application.Requests.Property;

public record UpdatePropertyRequest
{
    public Address? Address { get; init; }
    public decimal? RentPrice { get; init; }
    public int? Bedrooms { get; init; }
    public bool? IsAvailable { get; init; }
    public bool? IsActive { get; init; }
}
