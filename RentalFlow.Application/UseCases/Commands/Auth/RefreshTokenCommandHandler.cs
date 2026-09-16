namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class RefreshTokenCommandHandler(IUserTokenRepository _userTokenRepository, ITokenService _tokenService) : ICommandHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var token = await _userTokenRepository.GetByRefreshTokenAsync(command.Request.RefreshToken, cancellationToken);

        if (token is null || !token.IsValid())
        {
            return Result<LoginResponse>.Failure(UserErrors.InvalidRefreshToken);
        }

        if (!token.User.IsActive)
        {
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