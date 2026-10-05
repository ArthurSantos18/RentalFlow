namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class LoginCommandHandler(
    IUserRepository _userRepository,
    IUserTokenRepository _userTokenRepository,
    IPasswordService _passwordService,
    ITokenService _tokenService,
    ILogger<LoginCommandHandler> _logger
    ) : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = command.Request.Email;

        _logger.LogInformation(
            "Login attempt for {Email}",
            email);

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning(
                "Login failed for {Email} (user not found): {ErrorCode} {ErrorMessage}",
                email,
                UserErrors.InvalidCredentials.Code,
                UserErrors.InvalidCredentials.Message);

            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            _logger.LogWarning(
                "Login failed for user {UserId} ({Email}) because user is inactive: {ErrorCode} {ErrorMessage}",
                user.Id,
                user.Email,
                UserErrors.UserNotFound.Code,
                UserErrors.UserNotFound.Message);

            return Result<LoginResponse>.Failure(UserErrors.UserNotFound);
        }

        var passwordValid = _passwordService.Verify(command.Request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            _logger.LogWarning(
                "Login failed for user {UserId} ({Email}) due to invalid password: {ErrorCode} {ErrorMessage}",
                user.Id,
                user.Email,
                UserErrors.InvalidCredentials.Code,
                UserErrors.InvalidCredentials.Message);

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

        _logger.LogInformation(
            "User {UserId} ({Email}) logged in successfully with role {Role}",
            user.Id,
            user.Email,
            user.Operator?.Role.ToString() ?? "Broker");

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