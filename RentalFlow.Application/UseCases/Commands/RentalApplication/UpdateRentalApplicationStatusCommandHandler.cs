namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandler(
    IRentalApplicationRepository _rentalApplicationRepository,
    ICurrentUserService _currentUserService,
    ILogger<UpdateRentalApplicationStatusCommandHandler> _logger
    ) : ICommandHandler<UpdateRentalApplicationStatusCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateRentalApplicationStatusCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating status of rental application {RentalApplicationId} to {RentalStatus}",
            command.Id,
            command.Request.RentalStatus);

        var rentalApplication = await _rentalApplicationRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            _logger.LogWarning(
                "Rental application {RentalApplicationId} not found for status update: {ErrorCode} {ErrorMessage}",
                command.Id,
                RentalApplicationErrors.RentalApplicationNotFound.Code,
                RentalApplicationErrors.RentalApplicationNotFound.Message);

            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (!CanAccess(rentalApplication))
        {
            _logger.LogWarning(
                "Forbidden to update status of rental application {RentalApplicationId}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                UserErrors.Forbidden.Code,
                UserErrors.Forbidden.Message);

            return Result.Failure(UserErrors.Forbidden);
        }

        var previousStatus = rentalApplication.Status;

        var statusResult = rentalApplication.ChangeStatus(command.Request.RentalStatus);

        if (statusResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot change status of rental application {RentalApplicationId} from {PreviousStatus} to {NewStatus}: {ErrorCode} {ErrorMessage}",
                rentalApplication.Id,
                previousStatus,
                command.Request.RentalStatus,
                statusResult.Error.Code,
                statusResult.Error.Message);

            return statusResult;
        }

        await _rentalApplicationRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Status of rental application {RentalApplicationId} changed from {PreviousStatus} to {NewStatus} successfully",
            rentalApplication.Id,
            previousStatus,
            rentalApplication.Status);

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