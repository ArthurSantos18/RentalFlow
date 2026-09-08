using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandler(IRentalApplicationRepository _rentalApplicationrepository,
    IApplicantRepository _applicantRepository,
    IOperatorRepository _operatorRepository,
    IPropertyRepository _propertyRepository) : ICommandHandler<UpdateRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalApplicationrepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        var canBeEdited = rentalApplication.ValidateCanBeEdited();

        if (canBeEdited.IsFailure)
        {
            return canBeEdited;
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

        rentalApplication = command.Request.UpdateEntity(rentalApplication);

        _rentalApplicationrepository.Update(rentalApplication);

        await _rentalApplicationrepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
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

        var @operator = await _operatorRepository.GetByIdAsync(operatorId.Value, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        if (!@operator.IsActive)
        {
            return Result.Failure(OperatorErrors.OperatorIsInactive);
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
