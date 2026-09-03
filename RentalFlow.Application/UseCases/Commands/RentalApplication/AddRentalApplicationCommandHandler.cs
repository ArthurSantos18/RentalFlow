using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class AddRentalApplicationCommandHandler(IRentalApplicationRepository _rentalApplicationRepository,
    IApplicantRepository _applicantRepository,
    IPropertyRepository _propertyRepository,
    IOperatorRepository _operatorRepository) : ICommandHandler<AddRentalApplicationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByIdAsync(command.Request.ApplicantId, cancellationToken);

        if (applicant is null)
        {
            return Result<Guid>.Failure(ApplicantErrors.ApplicantNotFound);
        }

        var property = await _propertyRepository.GetByIdAsync(command.Request.PropertyId, cancellationToken);

        if (property is null)
        {
            return Result<Guid>.Failure(PropertyErrors.PropertyNotFound);
        }

        var @operator = await _operatorRepository.GetByIdAsync(command.Request.OperatorId, cancellationToken);

        if (@operator is null)
        {
            return Result<Guid>.Failure(OperatorErrors.OperatorNotFound);
        }

        var newRentalApplication = command.Request.ToEntity(applicant, property, @operator);

        await _rentalApplicationRepository.AddAsync(newRentalApplication, cancellationToken);

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(newRentalApplication.Id);
    }
}
