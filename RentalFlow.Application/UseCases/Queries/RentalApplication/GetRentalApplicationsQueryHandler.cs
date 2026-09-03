using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.RentalApplication;

public sealed class GetRentalApplicationsQueryHandler(
    IRentalApplicationRepository _repository
) : IQueryHandler<GetRentalApplicationsQuery, Result<PagedResult<GetRentalApplicationResponse>>>
{
    public async Task<Result<PagedResult<GetRentalApplicationResponse>>> HandleAsync(GetRentalApplicationsQuery query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetRentalApplicationsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetRentalApplicationResponse>>.Success(result.ToResponse());
    }
}
