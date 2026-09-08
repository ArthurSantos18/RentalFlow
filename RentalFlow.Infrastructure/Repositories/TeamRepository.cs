using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class TeamRepository(AppDbContext context) : BaseRepository<TeamEntity>(context), ITeamRepository
{
}
