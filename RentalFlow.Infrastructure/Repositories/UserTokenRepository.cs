namespace RentalFlow.Infrastructure.Repositories;

public sealed class UserTokenRepository(AppDbContext context) : BaseRepository<UserTokenEntity>(context), IUserTokenRepository
{
    public async Task<UserTokenEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(t => t.User).ThenInclude(u => u.Operator)
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var tokens = await _dbSet.Where(t => t.UserId == id && t.RevokedAt == null).ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }

    public async Task<int> CountActiveByUserIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .CountAsync(t => t.UserId == id && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }
}