namespace RentalFlow.Application.UseCases.Queries.Operator;

public sealed class GetOperatorsQueryHandler(
    IOperatorRepository _operatorRepository,
    IDataScopeService _dataScopeService
    ) : IQueryHandler<GetOperatorsQuery, Result<PagedResult<GetOperatorResponse>>>
{
    public async Task<Result<PagedResult<GetOperatorResponse>>> HandleAsync(GetOperatorsQuery query, CancellationToken cancellationToken)
    {
        var scope = _dataScopeService.GetScope();

        var result = await _operatorRepository.GetOperatorsAsync(query.Request, scope, cancellationToken);

        return Result<PagedResult<GetOperatorResponse>>.Success(result.ToResponse());
    }
}