namespace RentalFlow.Domain.Entities;

public sealed class UserTokenEntity : BaseEntity<UserTokenEntity>
{
    public Guid UserId { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; } = DateTime.UtcNow.AddDays(7);
    public DateTime? RevokedAt { get; private set; }
    public UserEntity User { get; private set; } = null!;

    public UserTokenEntity(
        UserEntity user,
        string refreshToken,
        DateTime expiresAt,
        DateTime createdAt,
        DateTime? revokedAt)
    {
        UserId = user.Id;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        RevokedAt = revokedAt;
        User = user;
    }

    private UserTokenEntity() { }

    public void Revoke()
    {
        RevokedAt ??= DateTime.UtcNow;
    }

    public bool IsValid()
    {
        return RevokedAt is null && ExpiresAt > DateTime.UtcNow;
    }
}