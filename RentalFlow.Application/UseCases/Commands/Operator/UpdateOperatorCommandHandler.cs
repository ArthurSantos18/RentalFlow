namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class UpdateOperatorCommandHandler(
    IOperatorRepository _operatorRepository,
    IUserRepository _userRepository,
    IUserTokenRepository _userTokenRepository,
    ICurrentUserService _currentUserService,
    ILogger<UpdateOperatorCommandHandler> _logger
    ) : ICommandHandler<UpdateOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateOperatorCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Updating operator {OperatorId} with email {Email}, role {Role}, active {IsActive}",
            command.Id,
            request.Email,
            request.Role,
            request.IsActive);

        var @operator = await _operatorRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

        if (@operator is null)
        {
            _logger.LogWarning(
                 "Operator {OperatorId} not found for update: {ErrorCode} {ErrorMessage}",
                 command.Id,
                 OperatorErrors.OperatorNotFound.Code,
                 OperatorErrors.OperatorNotFound.Message);

            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        var manageResult = EnsureCanManageOperator(@operator, request);

        if (manageResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot update operator {OperatorId}: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                manageResult.Error.Code,
                manageResult.Error.Message);

            return manageResult;
        }

        var adminResult = await EnsureAdminProtectionAsync(@operator, request, cancellationToken);

        if (adminResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot update operator {OperatorId} due to admin protection: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                adminResult.Error.Code,
                adminResult.Error.Message);

            return adminResult;
        }

        var emailResult = await UpdateEmailAsync(@operator, request.Email, cancellationToken);

        if (emailResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot update email for operator {OperatorId}: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                emailResult.Error.Code,
                emailResult.Error.Message);

            return emailResult;
        }

        @operator.UpdateFrom(request);

        if (request.IsActive == false && @operator.User is not null)
        {
            @operator.User.SetIsActive(false);
        }

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Operator {OperatorId} updated successfully",
            @operator.Id);

        return Result.Success();
    }

    private Result EnsureCanManageOperator(OperatorEntity @operator, UpdateOperatorRequest request)
    {
        if (_currentUserService.Role == nameof(OperatorRole.Administrator))
        {
            return Result.Success();
        }

        if (_currentUserService.Role != nameof(OperatorRole.Manager))
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (@operator.TeamId != _currentUserService.TeamId)
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (@operator.Role == OperatorRole.Administrator)
        {
            return Result.Failure(OperatorErrors.ManagerCannotManageAdmin);
        }

        if (request.Role == OperatorRole.Administrator)
        {
            return Result.Failure(OperatorErrors.CannotPromoteToAdmin);
        }

        return Result.Success();
    }

    private async Task<Result> EnsureAdminProtectionAsync(OperatorEntity @operator, UpdateOperatorRequest request, CancellationToken cancellationToken)
    {
        if (@operator.Role != OperatorRole.Administrator)
        {
            return Result.Success();
        }

        var isDeactivating = request.IsActive == false;
        var isDemoting = request.Role.HasValue && request.Role.Value != OperatorRole.Administrator;

        if (!isDeactivating && !isDemoting)
        {
            return Result.Success();
        }

        var adminCount = await _operatorRepository.CountActiveAdminsAsync(cancellationToken);

        if (adminCount <= 1)
        {
            return isDeactivating ? Result.Failure(OperatorErrors.CannotDeactivateLastAdmin) : Result.Failure(OperatorErrors.CannotDemoteLastAdmin);
        }

        return Result.Success();
    }

    private async Task<Result> UpdateEmailAsync(OperatorEntity @operator, string? newEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
        {
            return Result.Success();
        }

        if (@operator.User is null)
        {
            return Result.Success();
        }

        var normalized = newEmail.ToLowerInvariant();

        if (@operator.User.Email == normalized)
        {
            return Result.Success();
        }

        var emailExists = await _userRepository.EmailExistsAsync(normalized, cancellationToken);

        if (emailExists)
        {
            return Result.Failure(UserErrors.EmailAlreadyExists);
        }

        @operator.User.SetEmail(normalized);

        await _userTokenRepository.RevokeAllByUserIdAsync(@operator.User.Id, cancellationToken);

        return Result.Success();
    }
}