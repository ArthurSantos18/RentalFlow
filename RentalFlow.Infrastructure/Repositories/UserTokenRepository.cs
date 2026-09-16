using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.User;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class UserTokenRepository(AppDbContext context) : BaseRepository<UserTokenEntity>(context), IUserTokenRepository
{
    public async Task<UserTokenEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.User).ThenInclude(u => u.Operator)
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbSet.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }

    public async Task<int> CountActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task<PagedResult<UserTokenEntity>> GetTokensAsync(GetUserTokenRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Include(t => t.User).AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyUserIdsFilter(query, request.UserIds);
        query = ApplyRefreshTokensFilter(query, request.RefreshTokens);
        query = ApplyIsRevokedFilter(query, request.IsRevoked);
        query = ApplyIsExpiredFilter(query, request.IsExpired);
        query = ApplyCreatedAtFilter(query, request.MinCreatedAt, request.MaxCreatedAt);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserTokenEntity>(results, total, page, pageSize);
    }

    private static IQueryable<UserTokenEntity> ApplyIdsFilter(IQueryable<UserTokenEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(t => ids.Contains(t.Id));
        }

        return query;
    }

    private static IQueryable<UserTokenEntity> ApplyUserIdsFilter(IQueryable<UserTokenEntity> query, IEnumerable<Guid>? userIds)
    {
        if (userIds?.Any() == true)
        {
            return query.Where(t => userIds.Contains(t.UserId));
        }

        return query;
    }

    private static IQueryable<UserTokenEntity> ApplyRefreshTokensFilter(IQueryable<UserTokenEntity> query, IEnumerable<string>? refreshTokens)
    {
        if (refreshTokens?.Any() == true)
        {
            return query.Where(t => refreshTokens.Contains(t.RefreshToken));
        }

        return query;
    }

    private static IQueryable<UserTokenEntity> ApplyIsRevokedFilter(IQueryable<UserTokenEntity> query, bool? isRevoked)
    {
        if (isRevoked.HasValue)
        {
            return isRevoked.Value ? query.Where(t => t.RevokedAt != null) : query.Where(t => t.RevokedAt == null);
        }

        return query;
    }

    private static IQueryable<UserTokenEntity> ApplyIsExpiredFilter(IQueryable<UserTokenEntity> query, bool? isExpired)
    {
        if (isExpired.HasValue)
        {
            return isExpired.Value ? query.Where(t => t.ExpiresAt <= DateTime.UtcNow) : query.Where(t => t.ExpiresAt > DateTime.UtcNow);
        }

        return query;
    }

    private static IQueryable<UserTokenEntity> ApplyCreatedAtFilter(IQueryable<UserTokenEntity> query, DateTime? min, DateTime? max)
    {
        if (min.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= min.Value);
        }

        if (max.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= max.Value);
        }

        return query;
    }
}