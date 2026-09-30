namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class RefreshTokenCommandHandler(
    IUserTokenRepository _userTokenRepository,
    ITokenService _tokenService,
    ILogger<RefreshTokenCommandHandler> _logger
) : ICommandHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var token = await _userTokenRepository.GetByRefreshTokenAsync(command.Request.RefreshToken, cancellationToken);

        if (token is null || !token.IsValid())
        {
            _logger.LogWarning(
                "Refresh token failed (token not found or invalid/expired): {ErrorCode} {ErrorMessage}",
                UserErrors.InvalidRefreshToken.Code,
                UserErrors.InvalidRefreshToken.Message);

            return Result<LoginResponse>.Failure(UserErrors.InvalidRefreshToken);
        }

        if (!token.User.IsActive)
        {
            _logger.LogWarning(
                "Refresh token failed for user {UserId} ({Email}) because user is inactive: {ErrorCode} {ErrorMessage}",
                token.User.Id,
                token.User.Email,
                UserErrors.UserInactive.Code,
                UserErrors.UserInactive.Message);

            return Result<LoginResponse>.Failure(UserErrors.UserInactive);
        }

        token.Revoke();

        var newAccessToken = _tokenService.GenerateAccessToken(token.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var expiresAt = _tokenService.GetRefreshTokenExpiration();

        var newTokenEntity = new UserTokenEntity(
            token.User,
            newRefreshToken,
            expiresAt,
            DateTime.UtcNow,
            null);

        await _userTokenRepository.AddAsync(newTokenEntity, cancellationToken);
        await _userTokenRepository.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Refresh token rotated for user {UserId}",
            token.User.Id);

        var loginResponse = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            MustChangePassword = token.User.MustChangePassword,
            UserId = token.User.Id,
            Email = token.User.Email,
            Role = token.User.Operator?.Role.ToString() ?? "Broker"
        };

        return Result<LoginResponse>.Success(loginResponse);
    }
}