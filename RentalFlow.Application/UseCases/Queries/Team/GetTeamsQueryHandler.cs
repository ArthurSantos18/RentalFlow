namespace RentalFlow.Application.UseCases.Queries.Team;

public sealed class GetTeamsQueryHandler(ITeamRepository _teamRepository) : IQueryHandler<GetTeamsQuery, Result<PagedResult<GetTeamResponse>>>
{
    public async Task<Result<PagedResult<GetTeamResponse>>> HandleAsync(GetTeamsQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await _teamRepository.GetTeamsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetTeamResponse>>.Success(pagedResult.ToResponse());
    }
}
