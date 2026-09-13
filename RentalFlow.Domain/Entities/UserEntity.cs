namespace RentalFlow.Domain.Entities;

public sealed class UserEntity : BaseEntity<UserEntity>
{
    public Guid OperatorId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool MustChangePassword { get; private set; } = true;
    public OperatorEntity Operator { get; private set; } = null!;
    private readonly List<UserTokenEntity> _tokens = [];
    public IReadOnlyList<UserTokenEntity> Tokens => _tokens.AsReadOnly();

    public UserEntity(
        string email,
        string passwordHash,
        bool mustChangePassword,
        Guid operatorId,
        OperatorEntity @operator)
    {
        OperatorId = operatorId;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        MustChangePassword = mustChangePassword;
        Operator = @operator;
    }

    private UserEntity() { }

    public UserEntity SetEmail(string email)
    {
        Email = email.ToLowerInvariant();
        return this;
    }

    public UserEntity SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }

    public UserEntity SetOperator(OperatorEntity @operator)
    {
        Operator = @operator;
        OperatorId = @operator.Id;
        return this;
    }

    public UserEntity SetMustChangePassword(bool mustChangePassword)
    {
        MustChangePassword = mustChangePassword;
        return this;
    }

    public UserTokenEntity AddToken(string refreshToken, DateTime expiresAt)
    {
        var token = new UserTokenEntity(
            Id,
            refreshToken,
            expiresAt,
            DateTime.UtcNow,
            null);

        _tokens.Add(token);

        return token;
    }

    public void RevokeAllTokens()
    {
        foreach (var token in _tokens.Where(t => t.RevokedAt is null))
        {
            token.Revoke();
        }
    }

    public UserTokenEntity? GetActiveToken(string refreshToken)
    {
        return _tokens.FirstOrDefault(t => t.RefreshToken == refreshToken && t.IsValid());
    }
}