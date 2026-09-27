namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class AddRentalApplicationCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    IApplicantRepository _applicantRepository,
    IPropertyRepository _propertyRepository,
    IOperatorRepository _operatorRepository,
    ICurrentUserService _currentUserService
    ) : ICommandHandler<AddRentalApplicationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var operatorResult = await ResolveOperatorAsync(request.OperatorId, cancellationToken);

        if (operatorResult.IsFailure)
        {
            return Result<Guid>.Failure(operatorResult.Error);
        }

        var applicantResult = await GetValidApplicantAsync(request.ApplicantId, cancellationToken);

        if (applicantResult.IsFailure)
        {
            return Result<Guid>.Failure(applicantResult.Error);
        }

        var propertyResult = await GetValidPropertyAsync(request.PropertyId, cancellationToken);

        if (propertyResult.IsFailure)
        {
            return Result<Guid>.Failure(propertyResult.Error);
        }

        var rentalApplication = request.ToEntity(applicantResult.Value, propertyResult.Value, operatorResult.Value);

        await _rentalApplicationRepository.AddAsync(rentalApplication, cancellationToken);

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(rentalApplication.Id);
    }

    private async Task<Result<OperatorEntity>> ResolveOperatorAsync(Guid? requestedOperatorId, CancellationToken cancellationToken)
    {
        Guid operatorId;

        switch (_currentUserService.Role)
        {
            case nameof(OperatorRole.Administrator):
            case nameof(OperatorRole.Manager):
                operatorId = requestedOperatorId ?? _currentUserService.OperatorId;
                break;

            case nameof(OperatorRole.Broker):
                operatorId = _currentUserService.OperatorId;
                break;

            default:
                return Result<OperatorEntity>.Failure(UserErrors.InvalidRole);
        }

        var @operator = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);

        if (@operator is null)
        {
            return Result<OperatorEntity>.Failure(OperatorErrors.OperatorNotFound);
        }

        if (!@operator.IsActive)
        {
            return Result<OperatorEntity>.Failure(OperatorErrors.OperatorIsInactive);
        }

        if (_currentUserService.Role == nameof(OperatorRole.Manager) && @operator.TeamId != _currentUserService.TeamId)
        {
            return Result<OperatorEntity>.Failure(UserErrors.Forbidden);
        }

        return Result<OperatorEntity>.Success(@operator);
    }

    private async Task<Result<ApplicantEntity>> GetValidApplicantAsync(Guid id, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByIdAsync(id, cancellationToken);

        if (applicant is null)
        {
            return Result<ApplicantEntity>.Failure(ApplicantErrors.ApplicantNotFound);
        }

        if (!applicant.IsActive)
        {
            return Result<ApplicantEntity>.Failure(ApplicantErrors.ApplicantIsInactive);
        }

        return Result<ApplicantEntity>.Success(applicant);
    }

    private async Task<Result<PropertyEntity>> GetValidPropertyAsync(Guid id, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);

        if (property is null)
        {
            return Result<PropertyEntity>.Failure(PropertyErrors.PropertyNotFound);
        }

        if (!property.IsActive)
        {
            return Result<PropertyEntity>.Failure(PropertyErrors.PropertyIsInactive);
        }

        if (!property.IsAvailable)
        {
            return Result<PropertyEntity>.Failure(PropertyErrors.PropertyNotAvailable);
        }

        return Result<PropertyEntity>.Success(property);
    }
}