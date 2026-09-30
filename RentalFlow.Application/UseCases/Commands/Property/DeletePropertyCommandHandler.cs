namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed class DeletePropertyCommandHandler(
    IPropertyRepository _propertyRepository,
    IRentalApplicationRepository _rentalApplicationRepository,
    ILogger<DeletePropertyCommandHandler> _logger
    ) : ICommandHandler<DeletePropertyCommand, Result>
{
    public async Task<Result> HandleAsync(DeletePropertyCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting property {PropertyId}",
            command.Id);

        var property = await _propertyRepository.GetByIdAsync(command.Id, cancellationToken);

        if (property is null)
        {
            _logger.LogWarning(
                "Property {PropertyId} not found for deletion: {ErrorCode} {ErrorMessage}",
                command.Id,
                PropertyErrors.PropertyNotFound.Code,
                PropertyErrors.PropertyNotFound.Message);

            return Result.Failure(PropertyErrors.PropertyNotFound);
        }

        var hasApplications = await _rentalApplicationRepository.PropertyHasApplicationsAsync(property.Id, cancellationToken);

        if (hasApplications)
        {
            _logger.LogWarning(
                "Cannot delete property {PropertyId} ({Street}, {Number} - {City}) because it has associated rental applications: {ErrorCode} {ErrorMessage}",
                property.Id,
                property.Address.Street,
                property.Address.Number,
                property.Address.City,
                PropertyErrors.PropertyHasApplications.Code,
                PropertyErrors.PropertyHasApplications.Message);

            return Result.Failure(PropertyErrors.PropertyHasApplications);
        }

        property.MarkAsDeleted();

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Property {PropertyId} ({Street}, {Number} - {City}) deleted successfully",
            property.Id,
            property.Address.Street,
            property.Address.Number,
            property.Address.City);

        return Result.Success();
    }
}