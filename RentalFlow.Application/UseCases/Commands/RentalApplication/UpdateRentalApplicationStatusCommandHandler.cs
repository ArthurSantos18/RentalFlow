using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandler(IRentalApplicationRepository _rentalApplicationrepository) : ICommandHandler<UpdateRentalApplicationStatusCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationStatusCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalApplicationrepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        var statusResult = rentalApplication.ChangeStatus(command.Request.RentalStatus);

        if (statusResult.IsFailure)
        {
            return statusResult;
        }

        _rentalApplicationrepository.Update(rentalApplication);

        await _rentalApplicationrepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
