namespace RentalFlow.Application.Interfaces.Repositories;

public interface ITeamRepository : IBaseRepository<TeamEntity>
{
    Task<PagedResult<TeamEntity>> GetTeamsAsync(GetTeamRequest request, CancellationToken cancellationToken);
}
