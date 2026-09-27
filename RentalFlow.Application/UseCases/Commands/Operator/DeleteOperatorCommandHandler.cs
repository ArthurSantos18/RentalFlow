namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class DeleteOperatorCommandHandler(
    IOperatorRepository _operatorRepository,
    IUserTokenRepository _userTokenRepository,
    ICurrentUserService _currentUserService
    ) : ICommandHandler<DeleteOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteOperatorCommand command, CancellationToken cancellationToken)
    {
        var @operator = await _operatorRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        var permissionResult = await EnsureCanDeleteAsync(@operator, cancellationToken);

        if (permissionResult.IsFailure)
        {
            return permissionResult;
        }

        @operator.MarkAsDeleted();

        if (@operator.User is not null)
        {
            @operator.User.SetIsActive(false);

            await _userTokenRepository.RevokeAllByUserIdAsync(@operator.User.Id, cancellationToken);
        }

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> EnsureCanDeleteAsync(OperatorEntity @operator, CancellationToken cancellationToken)
    {
        if (@operator.Id == _currentUserService.OperatorId)
        {
            return Result.Failure(OperatorErrors.CannotDeleteSelf);
        }

        if (@operator.Role == OperatorRole.Administrator)
        {
            var adminCount = await _operatorRepository.CountActiveAdminsAsync(cancellationToken);

            if (adminCount <= 1)
            {
                return Result.Failure(OperatorErrors.CannotDeleteLastAdmin);
            }
        }

        return Result.Success();
    }
}