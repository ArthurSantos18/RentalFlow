using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Property;

namespace RentalFlow.Application.Mappers;

public static class PropertyMapper
{
    public static PropertyEntity ToEntity(this AddPropertyRequest request)
    {
        return new PropertyBuilder()
            .WithId(Guid.NewGuid())
            .WithAddress(request.Address)
            .WithBedrooms(request.Bedrooms)
            .WithRentPrice(request.RentPrice)
            .WithIsAvailable(request.IsAvaiable)
            .WithIsActive(true)
            .Build();
    }

    public static PropertyUpdate ToUpdateDomain(this UpdatePropertyRequest request)
    {
        return new PropertyUpdate(
            request.Address,
            request.RentPrice,
            request.Bedrooms,
            request.IsAvailable,
            request.IsActive);
    }

    public static GetPropertyResponse ToResponse(this PropertyEntity property)
    {
        return new GetPropertyResponse
        {
            Id = property.Id,
            Address = property.Address,
            RentPrice = property.RentPrice,
            Bedrooms = property.Bedrooms,
            IsAvailable = property.IsAvailable,
            IsActive = property.IsActive
        };
    }

    public static IEnumerable<GetPropertyResponse> ToResponse(this IEnumerable<PropertyEntity> properties)
    {
        return properties.Select(p => p.ToResponse());
    }
}
