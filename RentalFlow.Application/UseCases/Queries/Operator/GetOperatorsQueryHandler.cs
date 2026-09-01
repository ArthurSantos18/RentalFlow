using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Operator;

public sealed class GetOperatorsQueryHandler(IOperatorRepository _operatorRepository) : IQueryHandler<GetOperatorsQuery, Result<PagedResult<GetOperatorResponse>>>
{
    public async Task<Result<PagedResult<GetOperatorResponse>>> HandleAsync(GetOperatorsQuery query, CancellationToken cancellationToken)
    {
        var result = await _operatorRepository.GetOperatorsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetOperatorResponse>>.Success(result.ToResponse());
    }
}
