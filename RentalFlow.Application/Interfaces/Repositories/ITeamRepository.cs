using RentalFlow.Application.Requests.Team;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Interfaces.Repositories;

public interface ITeamRepository : IBaseRepository<TeamEntity>
{
    Task<PagedResult<TeamEntity>> GetTeamsAsync(GetTeamRequest request, CancellationToken cancellationToken);
}
