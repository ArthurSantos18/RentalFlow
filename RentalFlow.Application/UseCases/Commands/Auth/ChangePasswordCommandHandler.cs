namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class ChangePasswordCommandHandler(IUserRepository _userRepository, IPasswordService _passwordService) : ICommandHandler<ChangePasswordCommand, Result<string>>
{
    public async Task<Result<string>> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result<string>.Failure(UserErrors.UserNotFound);
        }

        var currentPasswordValid = _passwordService.Verify(command.Request.CurrentPassword, user.PasswordHash);

        if (!currentPasswordValid)
        {
            return Result<string>.Failure(UserErrors.InvalidCredentials);
        }

        var newPasswordSameAsCurrent = _passwordService.Verify(command.Request.NewPassword, user.PasswordHash);

        if (newPasswordSameAsCurrent)
        {
            return Result<string>.Failure(UserErrors.NewPasswordMustBeDifferent);
        }

        var newHash = _passwordService.Hash(command.Request.NewPassword);

        user.SetPasswordHash(newHash);

        user.SetMustChangePassword(false);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Password changed successfully.");
    }
}
