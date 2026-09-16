namespace RentalFlow.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByOperatorIdAsync(Guid operatorId, CancellationToken cancellationToken = default);
    Task<PagedResult<UserEntity>> GetUsersAsync(GetUserRequest request, CancellationToken cancellationToken = default);
}