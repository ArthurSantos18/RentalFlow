using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class ChangeRentalApplicationApplicantCommandHandler(
    IRentalApplicationRepository _rentalRepository,
    IApplicantRepository _applicantRepository
) : ICommandHandler<ChangeRentalApplicationApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(ChangeRentalApplicationApplicantCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (rentalApplication.Status != RentalStatus.Draft && rentalApplication.Status != RentalStatus.Pending)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationApplicantChangeNotAllowed);
        }

        var newApplicant = await _applicantRepository.GetByIdAsync(command.Request.NewApplicantId, cancellationToken);

        if (newApplicant is null)
        {
            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }
            
        if (!newApplicant.IsActive)
        {
            return Result.Failure(ApplicantErrors.ApplicantIsInactive);
        }

        if (rentalApplication.ApplicantId == newApplicant.Id)
        {
            return Result.Failure(RentalApplicationErrors.ApplicantAlreadyAssigned);
        }

        rentalApplication.SetApplicant(newApplicant);

        _rentalRepository.Update(rentalApplication);

        await _rentalRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
