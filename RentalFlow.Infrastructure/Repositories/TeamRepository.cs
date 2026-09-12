using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class TeamRepository(AppDbContext context) : BaseRepository<TeamEntity>(context), ITeamRepository
{
    public async Task<PagedResult<TeamEntity>> GetTeamsAsync(GetTeamRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Teams.AsNoTracking().AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyNamesFilter(query, request.Names);
        query = ApplyDescriptionFilter(query, request.Description);
        query = ApplyActiveFilter(query, request.IsActive);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TeamEntity>(results, total, page, pageSize);
    }

    private static IQueryable<TeamEntity> ApplyIdsFilter(IQueryable<TeamEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(t => ids.Contains(t.Id));
        }

        return query;
    }

    private static IQueryable<TeamEntity> ApplyNamesFilter(IQueryable<TeamEntity> query, IEnumerable<string>? names)
    {
        if (names?.Any() == true)
        {
            var namesLower = names.Select(n => n.ToLower()).ToList();
            return query.Where(t => namesLower.Contains(t.Name.ToLower()));
        }

        return query;
    }

    private static IQueryable<TeamEntity> ApplyDescriptionFilter(IQueryable<TeamEntity> query, IEnumerable<string>? names)
    {
        if (names?.Any() == true)
        {
            var namesLower = names.Select(n => n.ToLower()).ToList();
            return query.Where(t => namesLower.Contains(t.Description.ToLower()));
        }

        return query;
    }

    private static IQueryable<TeamEntity> ApplyActiveFilter(IQueryable<TeamEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(o => o.IsActive == isActive.Value);
        }

        return query;
    }
}
