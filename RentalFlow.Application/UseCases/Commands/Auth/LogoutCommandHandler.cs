namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed class LogoutCommandHandler(IUserTokenRepository _userTokenRepository) : ICommandHandler<LogoutCommand, Result>
{
    public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken cancellationToken)
    {
        var token = await _userTokenRepository.GetByRefreshTokenAsync(command.Request.RefreshToken, cancellationToken);

        if (token is null)
        {
            return Result.Success();
        }

        token.Revoke();

        await _userTokenRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}