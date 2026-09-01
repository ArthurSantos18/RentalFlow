using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Property;

public sealed class GetPropertiesQueryHandler(IPropertyRepository _propertyRepository) : IQueryHandler<GetPropertiesQuery, Result<PagedResult<GetPropertyResponse>>>
{
    public async Task<Result<PagedResult<GetPropertyResponse>>> HandleAsync(GetPropertiesQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await _propertyRepository.GetPropertiesAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetPropertyResponse>>.Success(pagedResult.ToResponse());
    }
}
