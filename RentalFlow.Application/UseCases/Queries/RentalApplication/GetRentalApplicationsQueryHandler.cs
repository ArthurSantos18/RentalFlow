namespace RentalFlow.Application.UseCases.Queries.RentalApplication;

public sealed class GetRentalApplicationsQueryHandler(IRentalApplicationRepository _repository) : IQueryHandler<GetRentalApplicationsQuery, Result<PagedResult<GetRentalApplicationResponse>>>
{
    public async Task<Result<PagedResult<GetRentalApplicationResponse>>> HandleAsync(GetRentalApplicationsQuery query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetRentalApplicationsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetRentalApplicationResponse>>.Success(result.ToResponse());
    }
}
