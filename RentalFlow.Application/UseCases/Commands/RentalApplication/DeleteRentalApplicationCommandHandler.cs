namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class DeleteRentalApplicationCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    ICurrentUserService _currentUserService,
    ILogger<DeleteRentalApplicationCommandHandler> _logger
    ) : ICommandHandler<DeleteRentalApplicationCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteRentalApplicationCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting rental application {RentalApplicationId}",
            command.Id);

        var rentalApplication = await _rentalApplicationRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            _logger.LogWarning(
                "Rental application {RentalApplicationId} not found for deletion: {ErrorCode} {ErrorMessage}",
                command.Id,
                RentalApplicationErrors.RentalApplicationNotFound.Code,
                RentalApplicationErrors.RentalApplicationNotFound.Message);

            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (!CanAccess(rentalApplication))
        {
            _logger.LogWarning(
                "Forbidden to delete rental application {RentalApplicationId}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                UserErrors.Forbidden.Code,
                UserErrors.Forbidden.Message);

            return Result.Failure(UserErrors.Forbidden);
        }

        if (!rentalApplication.IsActive)
        {
            _logger.LogInformation(
                "Rental application {RentalApplicationId} is already inactive, nothing to delete",
                rentalApplication.Id);

            return Result.Success();
        }

        rentalApplication.MarkAsDeleted();

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Rental application {RentalApplicationId} deleted successfully",
            rentalApplication.Id);

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