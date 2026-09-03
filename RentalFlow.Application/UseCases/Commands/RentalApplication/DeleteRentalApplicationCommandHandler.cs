using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class DeleteRentalApplicationCommandHandler(IRentalApplicationRepository _rentalApplicationRepository) : ICommandHandler<DeleteRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalApplicationRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (rentalApplication.IsActive == false)
        {
            return Result.Success();
        }

        rentalApplication.SetIsActive(false);

        _rentalApplicationRepository.Update(rentalApplication);

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
