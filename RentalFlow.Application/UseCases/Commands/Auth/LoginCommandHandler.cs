namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class LoginCommandHandler(
    IUserRepository _userRepository,
    IUserTokenRepository _userTokenRepository,
    IPasswordService _passwordService,
    ITokenService _tokenService
) : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Request.Email, cancellationToken);

        if (user is null)
        {
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return Result<LoginResponse>.Failure(UserErrors.UserInactive);
        }

        var passwordValid = _passwordService.Verify(command.Request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
        }

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var expiresAt = _tokenService.GetRefreshTokenExpiration();

        var tokenEntity = new UserTokenEntity(
            user,
            refreshToken,
            expiresAt,
            DateTime.UtcNow,
            null);

        await _userTokenRepository.AddAsync(tokenEntity, cancellationToken);

        await _userTokenRepository.SaveChangesAsync(cancellationToken);

        var loginResponse = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            MustChangePassword = user.MustChangePassword,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Operator?.Role.ToString() ?? "Broker"

        };

        return Result<LoginResponse>.Success(loginResponse);
    }
}
