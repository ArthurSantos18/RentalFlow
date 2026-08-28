using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Domain.Entities.Property;

public record PropertyUpdate(
    Address? Address,
    decimal? RentPrice,
    int? Bedrooms,
    bool? IsAvailable,
    bool? IsActive);
