namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class ChangePasswordCommandHandler(
    IUserRepository _userRepository,
    IPasswordService _passwordService,
    ILogger<ChangePasswordCommandHandler> _logger
    ) : ICommandHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Password change requested for user {UserId}",
            command.UserId);

        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning(
                "Password change failed because user {UserId} was not found: {ErrorCode} {ErrorMessage}",
                command.UserId,
                UserErrors.UserNotFound.Code,
                UserErrors.UserNotFound.Message);

            return Result.Failure(UserErrors.UserNotFound);
        }

        var currentPasswordValid = _passwordService.Verify(command.Request.CurrentPassword, user.PasswordHash);

        if (!currentPasswordValid)
        {
            _logger.LogWarning(
                "Password change failed for user {UserId} ({Email}) because current password is invalid: {ErrorCode} {ErrorMessage}",
                user.Id,
                user.Email,
                UserErrors.InvalidPassword.Code,
                UserErrors.InvalidPassword.Message);

            return Result.Failure(UserErrors.InvalidPassword);
        }

        var newHash = _passwordService.Hash(command.Request.NewPassword);

        user.SetPasswordHash(newHash);

        user.SetMustChangePassword(false);

        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Password changed successfully for user {UserId} ({Email})",
            user.Id,
            user.Email);

        return Result.Success();
    }
}