using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandler(IRentalApplicationRepository _rentalApplicationrepository) : ICommandHandler<UpdateRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalApplicationrepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        var update = command.Request.ToUpdateDomain();

        rentalApplication.Update(update);

        _rentalApplicationrepository.Update(rentalApplication);

        await _rentalApplicationrepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
