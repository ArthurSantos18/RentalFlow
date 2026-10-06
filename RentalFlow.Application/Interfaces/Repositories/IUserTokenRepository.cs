namespace RentalFlow.Application.Interfaces.Repositories;

public interface IUserTokenRepository : IBaseRepository<UserTokenEntity>
{
    Task<UserTokenEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeAllByUserIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> CountActiveByUserIdAsync(Guid id, CancellationToken cancellationToken);
}