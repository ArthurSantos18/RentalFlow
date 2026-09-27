namespace RentalFlow.Application.UseCases.Queries.RentalApplication;

public sealed class GetRentalApplicationsQueryHandler(IRentalApplicationRepository _repository, IDataScopeService _dataScopeService) : IQueryHandler<GetRentalApplicationsQuery, Result<PagedResult<GetRentalApplicationResponse>>>
{
    public async Task<Result<PagedResult<GetRentalApplicationResponse>>> HandleAsync(GetRentalApplicationsQuery query, CancellationToken cancellationToken)
    {
        var scope = _dataScopeService.GetScope();

        var result = await _repository.GetRentalApplicationsAsync(query.Request, scope, cancellationToken);

        return Result<PagedResult<GetRentalApplicationResponse>>.Success(result.ToResponse());
    }
}
