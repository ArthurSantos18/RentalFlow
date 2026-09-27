namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    IApplicantRepository _applicantRepository,
    IOperatorRepository _operatorRepository,
    IPropertyRepository _propertyRepository,
    ICurrentUserService _currentUserService
    ) : ICommandHandler<UpdateRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalApplicationRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (!CanAccess(rentalApplication))
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        var canEdit = EnsureCanBeEdited(rentalApplication);

        if (canEdit.IsFailure)
        {
            return canEdit;
        }

        var applicantResult = await UpdateApplicantAsync(rentalApplication, command.Request.ApplicantId, cancellationToken);

        if (applicantResult.IsFailure)
        {
            return applicantResult;
        }

        var operatorResult = await UpdateOperatorAsync(rentalApplication, command.Request.OperatorId, cancellationToken);

        if (operatorResult.IsFailure)
        {
            return operatorResult;
        }

        var propertyResult = await UpdatePropertyAsync(rentalApplication, command.Request.PropertyId, cancellationToken);

        if (propertyResult.IsFailure)
        {
            return propertyResult;
        }

        rentalApplication.UpdateFrom(command.Request);

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

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
            return Result.Failure(ApplicantErrors.ApplicantIsInactive);
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
            return Result.Failure(UserErrors.Forbidden);
        }

        var @operator = await _operatorRepository.GetByIdAsync(operatorId.Value, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        if (!@operator.IsActive)
        {
            return Result.Failure(OperatorErrors.OperatorIsInactive);
        }

        if (_currentUserService.Role == nameof(OperatorRole.Manager) && @operator.TeamId != _currentUserService.TeamId)
        {
            return Result.Failure(UserErrors.Forbidden);
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
            return Result.Failure(PropertyErrors.PropertyIsInactive);
        }

        if (!property.IsAvailable)
        {
            return Result.Failure(PropertyErrors.PropertyNotAvailable);
        }

        return rentalApplication.ChangeProperty(property);
    }
}