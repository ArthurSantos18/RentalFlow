namespace RentalFlow.Infrastructure.Repositories;

public sealed class OperatorRepository(AppDbContext context) : BaseRepository<OperatorEntity>(context), IOperatorRepository
{
    public async Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.CountAsync(o => o.Role == OperatorRole.Administrator && o.IsActive && !o.IsDeleted, cancellationToken);
    }

    public async Task<int> CountActiveByTeamAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.CountAsync(o => o.TeamId == id && o.IsActive && !o.IsDeleted, cancellationToken);
    }

    public async Task<OperatorEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.Include(o => o.Team).Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<PagedResult<OperatorEntity>> GetOperatorsAsync(GetOperatorRequest request, DataScope scope, CancellationToken cancellationToken)
    {
        var query = _context.Operators
            .AsNoTracking()
            .Include(o => o.Team)
            .Include(o => o.User)
            .AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyNamesFilter(query, request.Names);
        query = ApplyRoleFilter(query, request.Role);
        query = ApplyActiveFilter(query, request.IsActive);
        query = ApplyHasApplicationsFilter(query, request.HasApplications);
        query = ApplyApplicationIdsFilter(query, request.ApplicationIds);
        query = ApplyTeamIdsFilter(query, request.TeamIds);
        query = ApplyDataScope(query, scope);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OperatorEntity>(results, total, page, pageSize);
    }

    private static IQueryable<OperatorEntity> ApplyIdsFilter(IQueryable<OperatorEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyNamesFilter(IQueryable<OperatorEntity> query, IEnumerable<string>? names)
    {
        if (names?.Any() == true)
        {
            var namesLower = names.Select(n => n.ToLower()).ToList();
            return query.Where(o => namesLower.Contains(o.Name.ToLower()));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyRoleFilter(IQueryable<OperatorEntity> query, OperatorRole? role)
    {
        if (role.HasValue && role != OperatorRole.None)
        {
            return query.Where(o => o.Role == role.Value);
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyActiveFilter(IQueryable<OperatorEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(o => o.IsActive == isActive.Value);
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyHasApplicationsFilter(IQueryable<OperatorEntity> query, bool? hasApplications)
    {
        if (hasApplications.HasValue)
        {
            return hasApplications.Value ? query.Where(o => o.Applications.Any()) : query.Where(o => !o.Applications.Any());
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyApplicationIdsFilter(IQueryable<OperatorEntity> query, IEnumerable<Guid>? applicationIds)
    {
        if (applicationIds?.Any() == true)
        {
            return query.Where(o => o.Applications.Any(a => applicationIds.Contains(a.Id)));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyTeamIdsFilter(IQueryable<OperatorEntity> query, IEnumerable<Guid>? teamIds)
    {
        if (teamIds?.Any() == true)
        {
            return query.Where(o => teamIds.Contains(o.TeamId));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyDataScope(IQueryable<OperatorEntity> query, DataScope scope)
    {
        if (scope.IsGlobal)
        {
            return query;
        }

        if (scope.OperatorIds is null && scope.TeamIds is null)
        {
            return query.Where(_ => false);
        }

        if (scope.TeamIds?.Any() == true)
        {
            query = query.Where(o => scope.TeamIds.Contains(o.TeamId));
        }

        return query;
    }
}