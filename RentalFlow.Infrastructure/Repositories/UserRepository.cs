namespace RentalFlow.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context) : BaseRepository<UserEntity>(context), IUserRepository
{
    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(u => u.Operator).ThenInclude(o => o.Team)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<UserEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Operator).ThenInclude(o => o.Team)
            .FirstOrDefaultAsync(u => _context.UserTokens.Any(t => t.UserId == u.Id && t.RefreshToken == refreshToken), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<UserEntity?> GetByOperatorIdAsync(Guid operatorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(u => u.Operator).FirstOrDefaultAsync(u => u.OperatorId == operatorId, cancellationToken);
    }

    public async Task<PagedResult<UserEntity>> GetUsersAsync(GetUserRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyEmailsFilter(query, request.Emails);
        query = ApplyOperatorIdsFilter(query, request.OperatorIds);
        query = ApplyIsActiveFilter(query, request.IsActive);
        query = ApplyMustChangePasswordFilter(query, request.MustChangePassword);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(u => u.Operator)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserEntity>(results, total, page, pageSize);
    }

    private static IQueryable<UserEntity> ApplyIdsFilter(IQueryable<UserEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(u => ids.Contains(u.Id));
        }
            
        return query;
    }

    private static IQueryable<UserEntity> ApplyEmailsFilter(IQueryable<UserEntity> query, IEnumerable<string>? emails)
    {
        if (emails?.Any() == true)
        {
            var emailsLower = emails.Select(e => e.ToLowerInvariant()).ToList();
            return query.Where(u => emailsLower.Contains(u.Email));
        }

        return query;
    }

    private static IQueryable<UserEntity> ApplyOperatorIdsFilter(IQueryable<UserEntity> query, IEnumerable<Guid>? operatorIds)
    {
        if (operatorIds?.Any() == true)
        {
            return query.Where(u => operatorIds.Contains(u.OperatorId));
        }
            
        return query;
    }

    private static IQueryable<UserEntity> ApplyIsActiveFilter(IQueryable<UserEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(u => u.IsActive == isActive.Value);
        }
            
        return query;
    }

    private static IQueryable<UserEntity> ApplyMustChangePasswordFilter(IQueryable<UserEntity> query, bool? mustChangePassword)
    {
        if (mustChangePassword.HasValue)
        {
            return query.Where(u => u.MustChangePassword == mustChangePassword.Value);
        }
            
        return query;
    }
}
