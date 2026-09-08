using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Patterns.PagedResult;

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
            .WithIsAvailable(request.IsAvailable)
            .WithIsActive(true)
            .Build();
    }

    public static PropertyEntity UpdateEntity(this UpdatePropertyRequest request, PropertyEntity entity)
    {
        return entity
            .SetAddress(request.Address ?? entity.Address)
            .SetBedrooms(request.Bedrooms ?? entity.Bedrooms)
            .SetRentPrice(request.RentPrice ?? entity.RentPrice)
            .SetIsAvailable(request.IsAvailable ?? entity.IsAvailable);
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

    public static PagedResult<GetPropertyResponse> ToResponse(this PagedResult<PropertyEntity> pagedResult)
    {
        return new PagedResult<GetPropertyResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(p => p.ToResponse())
        };
    }
}
