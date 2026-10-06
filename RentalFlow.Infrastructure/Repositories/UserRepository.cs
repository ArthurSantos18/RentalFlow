#pragma warning disable CA1862

namespace RentalFlow.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context) : BaseRepository<UserEntity>(context), IUserRepository
{
    public async Task<UserEntity?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.Include(u => u.Operator).ThenInclude(o => o.Team)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.Include(u => u.Operator).ThenInclude(o => o.Team)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<UserEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(u => u.Operator).ThenInclude(o => o.Team)
            .FirstOrDefaultAsync(u => _context.UserTokens.Any(t => t.UserId == u.Id && t.RefreshToken == refreshToken), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<UserEntity?> GetByOperatorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.Include(u => u.Operator).FirstOrDefaultAsync(u => u.OperatorId == id, cancellationToken);
    }
}