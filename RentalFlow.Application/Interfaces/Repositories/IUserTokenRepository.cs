namespace RentalFlow.Application.Interfaces.Repositories;

public interface IUserTokenRepository : IBaseRepository<UserTokenEntity>
{
    Task<UserTokenEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> CountActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserTokenEntity>> GetTokensAsync(GetUserTokenRequest request, CancellationToken cancellationToken = default);
}
