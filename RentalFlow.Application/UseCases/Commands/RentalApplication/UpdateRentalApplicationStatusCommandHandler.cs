namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    ICurrentUserService _currentUserService
) : ICommandHandler<UpdateRentalApplicationStatusCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationStatusCommand command, CancellationToken cancellationToken)
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

        var statusResult = rentalApplication.ChangeStatus(command.Request.RentalStatus);

        if (statusResult.IsFailure)
        {
            return statusResult;
        }

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private bool CanAccess(RentalApplicationEntity rentalApplication)
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Administrator) => true,

            nameof(OperatorRole.Manager) => rentalApplication.Operator.TeamId == _currentUserService.TeamId,

            nameof(OperatorRole.Broker) => false,

            _ => false
        };
    }
}