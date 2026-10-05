namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    IApplicantRepository _applicantRepository,
    IOperatorRepository _operatorRepository,
    IPropertyRepository _propertyRepository,
    ICurrentUserService _currentUserService,
    ILogger<UpdateRentalApplicationCommandHandler> _logger
    ) : ICommandHandler<UpdateRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating rental application {RentalApplicationId}",
            command.Id);

        var rentalApplication = await _rentalApplicationRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            _logger.LogWarning(
                "Rental application {RentalApplicationId} not found for update: {ErrorCode} {ErrorMessage}",
                command.Id,
                RentalApplicationErrors.RentalApplicationNotFound.Code,
                RentalApplicationErrors.RentalApplicationNotFound.Message);

            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (!CanAccess(rentalApplication))
        {
            _logger.LogWarning(
                "Forbidden to update rental application {RentalApplicationId}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                UserErrors.InvalidRole.Code,
                UserErrors.InvalidRole.Message);

            return Result.Failure(UserErrors.InvalidRole);
        }

        var canEdit = EnsureCanBeEdited(rentalApplication);

        if (canEdit.IsFailure)
        {
            _logger.LogWarning(
                "Rental application {RentalApplicationId} cannot be edited: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                canEdit.Error.Code,
                canEdit.Error.Message);

            return canEdit;
        }

        var applicantResult = await UpdateApplicantAsync(rentalApplication, command.Request.ApplicantId, cancellationToken);

        if (applicantResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot update applicant for rental application {RentalApplicationId}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                applicantResult.Error.Code,
                applicantResult.Error.Message);

            return applicantResult;
        }

        var operatorResult = await UpdateOperatorAsync(rentalApplication, command.Request.OperatorId, cancellationToken);

        if (operatorResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot update operator for rental application {RentalApplicationId}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                operatorResult.Error.Code,
                operatorResult.Error.Message);

            return operatorResult;
        }

        var propertyResult = await UpdatePropertyAsync(rentalApplication, command.Request.PropertyId, cancellationToken);

        if (propertyResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot update property for rental application {RentalApplicationId}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                propertyResult.Error.Code,
                propertyResult.Error.Message);

            return propertyResult;
        }

        rentalApplication.UpdateFrom(command.Request);

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Rental application {RentalApplicationId} updated successfully",
            rentalApplication.Id);

        return Result.Success();
    }

    private bool CanAccess(RentalApplicationEntity rentalApplication)
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Administrator) => true,

            nameof(OperatorRole.Manager) => rentalApplication.Operator?.TeamId == _currentUserService.TeamId,

            nameof(OperatorRole.Broker) => rentalApplication.OperatorId == _currentUserService.OperatorId,

            _ => false
        };
    }

    private Result EnsureCanBeEdited(RentalApplicationEntity rentalApplication)
    {
        if (_currentUserService.Role == nameof(OperatorRole.Broker))
        {
            return rentalApplication.Status == RentalStatus.Draft ? Result.Success() : Result.Failure(RentalApplicationErrors.RentalApplicationCannotBeEdited);
        }

        return rentalApplication.ValidateCanBeEdited();
    }

    private async Task<Result> UpdateApplicantAsync(RentalApplicationEntity rentalApplication, Guid? applicantId, CancellationToken cancellationToken)
    {
        if (applicantId is null)
        {
            return Result.Success();
        }

        var applicant = await _applicantRepository.GetByIdAsync(applicantId.Value, cancellationToken);

        if (applicant is null)
        {
            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        if (!applicant.IsActive)
        {
            return Result.Failure(ApplicantErrors.ApplicantInactive);
        }

        return rentalApplication.ChangeApplicant(applicant);
    }

    private async Task<Result> UpdateOperatorAsync(RentalApplicationEntity rentalApplication, Guid? operatorId, CancellationToken cancellationToken)
    {
        if (operatorId is null)
        {
            return Result.Success();
        }

        if (_currentUserService.Role == nameof(OperatorRole.Broker))
        {
            return Result.Failure(UserErrors.InvalidRole);
        }

        var @operator = await _operatorRepository.GetByIdAsync(operatorId.Value, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        if (!@operator.IsActive)
        {
            return Result.Failure(OperatorErrors.OperatorInactive);
        }

        if (_currentUserService.Role == nameof(OperatorRole.Manager) && @operator.TeamId != _currentUserService.TeamId)
        {
            return Result.Failure(UserErrors.InvalidRole);
        }

        return rentalApplication.ChangeOperator(@operator);
    }

    private async Task<Result> UpdatePropertyAsync(RentalApplicationEntity rentalApplication, Guid? propertyId, CancellationToken cancellationToken)
    {
        if (propertyId is null)
        {
            return Result.Success();
        }

        var property = await _propertyRepository.GetByIdAsync(propertyId.Value, cancellationToken);

        if (property is null)
        {
            return Result.Failure(PropertyErrors.PropertyNotFound);
        }

        if (!property.IsActive)
        {
            return Result.Failure(PropertyErrors.PropertyInactive);
        }

        if (!property.IsAvailable)
        {
            return Result.Failure(PropertyErrors.PropertyNotAvailable);
        }

        return rentalApplication.ChangeProperty(property);
    }
}