namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class DeleteOperatorCommandHandler(
    IOperatorRepository _operatorRepository,
    IUserTokenRepository _userTokenRepository,
    ICurrentUserService _currentUserService,
    ILogger<DeleteOperatorCommandHandler> _logger
    ) : ICommandHandler<DeleteOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteOperatorCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting operator {OperatorId}",
            command.Id);

        var @operator = await _operatorRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

        if (@operator is null)
        {
            _logger.LogWarning(
                "Operator {OperatorId} not found for deletion: {ErrorCode} {ErrorMessage}",
                command.Id,
                OperatorErrors.OperatorNotFound.Code,
                OperatorErrors.OperatorNotFound.Message);

            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        var permissionResult = await EnsureCanDeleteAsync(@operator, cancellationToken);

        if (permissionResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot delete operator {OperatorId}: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                permissionResult.Error.Code,
                permissionResult.Error.Message);

            return permissionResult;
        }

        @operator.MarkAsDeleted();

        if (@operator.User is not null)
        {
            @operator.User.SetIsActive(false);

            await _userTokenRepository.RevokeAllByUserIdAsync(@operator.User.Id, cancellationToken);
        }

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Operator {OperatorId} deleted successfully",
            @operator.Id);

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