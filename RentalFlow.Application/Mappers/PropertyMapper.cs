namespace RentalFlow.Application.Mappers;

public static class PropertyMapper
{
    public static PropertyEntity ToEntity(this AddPropertyRequest request)
    {
        return new PropertyEntity(
            address: request.Address,
            rentPrice: request.RentPrice,
            bedrooms: request.Bedrooms,
            isAvailable: request.IsAvailable);
    }

    public static PropertyEntity UpdateFrom(this PropertyEntity entity, UpdatePropertyRequest request)
    {
        if (request.Address is not null)
        {
            entity.SetAddress(request.Address);
        }

        if (request.RentPrice.HasValue)
        {
            entity.SetRentPrice(request.RentPrice.Value);
        }

        if (request.Bedrooms.HasValue)
        {
            entity.SetBedrooms(request.Bedrooms.Value);
        }

        if (request.IsAvailable.HasValue)
        {
            entity.SetIsAvailable(request.IsAvailable.Value);
        }

        if (request.IsActive.HasValue)
        {
            entity.SetIsActive(request.IsActive.Value);
        }

        return entity.Touch();
    }

    public static GetPropertyResponse ToResponse(this PropertyEntity entity)
    {
        return new GetPropertyResponse
        {
            Id = entity.Id,
            Address = entity.Address,
            RentPrice = entity.RentPrice,
            Bedrooms = entity.Bedrooms,
            IsAvailable = entity.IsAvailable,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
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
