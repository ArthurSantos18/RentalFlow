namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class DeleteRentalApplicationCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    ICurrentUserService _currentUserService
    ) : ICommandHandler<DeleteRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteRentalApplicationCommand command, CancellationToken cancellationToken)
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

        if (!rentalApplication.IsActive)
        {
            return Result.Success();
        }

        rentalApplication.MarkAsDeleted();

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private bool CanAccess(RentalApplicationEntity rentalApplication)
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Administrator) => true,

            nameof(OperatorRole.Manager) => rentalApplication.Operator?.TeamId == _currentUserService.TeamId,

            _ => false
        };
    }
}