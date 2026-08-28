using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Property;

public sealed class GetPropertiesQueryHandler(IPropertyRepository _propertyRepository) : IQueryHandler<GetPropertiesQuery, Result<IEnumerable<GetPropertyResponse>>>
{
    public async Task<Result<IEnumerable<GetPropertyResponse>>> HandleAsync(GetPropertiesQuery query, CancellationToken cancellationToken)
    {
        var result = await _propertyRepository.GetAllAsync(cancellationToken);

        return Result<IEnumerable<GetPropertyResponse>>.Success(result.ToResponse());
    }
}
