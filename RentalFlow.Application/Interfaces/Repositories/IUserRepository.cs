namespace RentalFlow.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<UserEntity?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserEntity?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task<UserEntity?> GetByOperatorIdAsync(Guid id, CancellationToken cancellationToken);
}