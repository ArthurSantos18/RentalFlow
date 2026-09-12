using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Team;

public sealed class GetPropertiesQueryHandler(ITeamRepository _teamRepository) : IQueryHandler<GetTeamsQuery, Result<PagedResult<GetTeamResponse>>>
{
    public async Task<Result<PagedResult<GetTeamResponse>>> HandleAsync(GetTeamsQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await _teamRepository.GetTeamsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetTeamResponse>>.Success(pagedResult.ToResponse());
    }
}
