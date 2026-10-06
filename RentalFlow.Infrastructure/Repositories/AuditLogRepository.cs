namespace RentalFlow.Infrastructure.Repositories;

public sealed class AuditLogRepository(AppDbContext context) : IAuditLogRepository
{
    private readonly DbSet<AuditLogEntity> _dbSet = context.Set<AuditLogEntity>();

    public async Task<PagedResult<AuditLogEntity>> GetAuditLogsAsync(GetAuditLogRequest request, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking().Where(a => a.EntityName == request.EntityName.ToString() && a.EntityId == request.EntityId);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;

        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogEntity>(results, total, page, pageSize);
    }
}