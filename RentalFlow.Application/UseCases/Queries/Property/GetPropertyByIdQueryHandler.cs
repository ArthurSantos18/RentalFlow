using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Property;

public sealed class GetPropertyByIdQueryHandler(IPropertyRepository _propertyRepository) : IQueryHandler<GetPropertyByIdQuery, Result<GetPropertyResponse>>
{
    public async Task<Result<GetPropertyResponse>> HandleAsync(GetPropertyByIdQuery query, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(query.Id, cancellationToken);

        if (property == null)
        {
            return Result<GetPropertyResponse>.Failure(PropertyErrors.PropertyNotFound);
        }

        return Result<GetPropertyResponse>.Success(property.ToResponse());
    }
}
