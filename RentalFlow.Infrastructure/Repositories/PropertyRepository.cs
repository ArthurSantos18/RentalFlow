using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class PropertyRepository(AppDbContext context) : BaseRepository<PropertyEntity>(context), IPropertyRepository
{
    public async Task<PagedResult<PropertyEntity>> GetPropertiesAsync(GetPropertyRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Properties.AsNoTracking().AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyCitiesFilter(query, request.Cities);
        query = ApplyStatesFilter(query, request.States);
        query = ApplyNeighborhoodsFilter(query, request.Neighborhoods);
        query = ApplyZipCodesFilter(query, request.ZipCodes);
        query = ApplyRentPriceFilter(query, request.MinRentPrice, request.MaxRentPrice);
        query = ApplyBedroomsFilter(query, request.MinBedrooms, request.MaxBedrooms);
        query = ApplyAvailabilityFilter(query, request.IsAvailable);
        query = ApplyActiveFilter(query, request.IsActive);
        query = ApplyHasApplicationsFilter(query, request.HasApplications);
        query = ApplyApplicationIdsFilter(query, request.ApplicationIds);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PropertyEntity>(results, total, page, pageSize);
    }

    private static IQueryable<PropertyEntity> ApplyIdsFilter(IQueryable<PropertyEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(p => ids.Contains(p.Id));
        }
            
        return query;
    }

    private static IQueryable<PropertyEntity> ApplyCitiesFilter(IQueryable<PropertyEntity> query, IEnumerable<string>? cities)
    {
        if (cities?.Any() == true)
        {
            var citiesLower = cities.Select(c => c.ToLower()).ToList();
            return query.Where(p => citiesLower.Contains(p.Address.City.ToLower()));
        }

        return query;
    }

    private static IQueryable<PropertyEntity> ApplyStatesFilter(IQueryable<PropertyEntity> query, IEnumerable<string>? states)
    {
        if (states?.Any() == true)
        {
            var statesUpper = states.Select(s => s.ToUpper()).ToList();
            return query.Where(p => statesUpper.Contains(p.Address.State.ToUpper()));
        }

        return query;
    }

    private static IQueryable<PropertyEntity> ApplyNeighborhoodsFilter(IQueryable<PropertyEntity> query, IEnumerable<string>? neighborhoods)
    {
        if (neighborhoods?.Any() == true)
        {
            var neighborhoodsLower = neighborhoods.Select(n => n.ToLower()).ToList();
            return query.Where(p => neighborhoodsLower.Contains(p.Address.Neighborhood.ToLower()));
        }
        return query;
    }

    private static IQueryable<PropertyEntity> ApplyZipCodesFilter(IQueryable<PropertyEntity> query, IEnumerable<string>? zipCodes)
    {
        if (zipCodes?.Any() == true)
        {
            var zipCodesClean = zipCodes.Select(z => z.Replace("-", "")).ToList();
            return query.Where(p => zipCodesClean.Contains(p.Address.ZipCode.Replace("-", "")));
        }
        return query;
    }

    private static IQueryable<PropertyEntity> ApplyRentPriceFilter(IQueryable<PropertyEntity> query, decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.RentPrice >= minPrice.Value);
        }
            
        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.RentPrice <= maxPrice.Value);
        }
            
        return query;
    }

    private static IQueryable<PropertyEntity> ApplyBedroomsFilter(IQueryable<PropertyEntity> query, int? minBedrooms, int? maxBedrooms)
    {
        if (minBedrooms.HasValue)
        {
            query = query.Where(p => p.Bedrooms >= minBedrooms.Value);
        }

        if (maxBedrooms.HasValue)
        {
            query = query.Where(p => p.Bedrooms <= maxBedrooms.Value);
        }

        return query;
    }

    private static IQueryable<PropertyEntity> ApplyAvailabilityFilter(IQueryable<PropertyEntity> query, bool? isAvailable)
    {
        if (isAvailable.HasValue)
        {
            return query.Where(p => p.IsAvailable == isAvailable.Value);
        }

        return query;
    }

    private static IQueryable<PropertyEntity> ApplyActiveFilter(IQueryable<PropertyEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(p => p.IsActive == isActive.Value);
        }

        return query;
    }

    private static IQueryable<PropertyEntity> ApplyHasApplicationsFilter(IQueryable<PropertyEntity> query, bool? hasApplications)
    {
        if (hasApplications.HasValue)
        {
            return hasApplications.Value ? query.Where(p => p.Applications.Any()) : query.Where(p => !p.Applications.Any());
        }
        return query;
    }

    private static IQueryable<PropertyEntity> ApplyApplicationIdsFilter(IQueryable<PropertyEntity> query, IEnumerable<Guid>? applicationIds)
    {
        if (applicationIds?.Any() == true)
        {
            return query.Where(p => p.Applications.Any(a => applicationIds.Contains(a.Id)));
        }
        return query;
    }
}
