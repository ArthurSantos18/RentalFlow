namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed class UpdatePropertyCommandHandler(
    IPropertyRepository _propertyRepository,
    ILogger<UpdatePropertyCommandHandler> _logger
    ) : ICommandHandler<UpdatePropertyCommand, Result>
{
    public async Task<Result> HandleAsync(UpdatePropertyCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating property {PropertyId}",
            command.Id);

        var property = await _propertyRepository.GetByIdAsync(command.Id, cancellationToken);

        if (property is null)
        {
            _logger.LogWarning(
                "Property {PropertyId} not found for update: {ErrorCode} {ErrorMessage}",
                command.Id,
                PropertyErrors.PropertyNotFound.Code,
                PropertyErrors.PropertyNotFound.Message);

            return Result.Failure(PropertyErrors.PropertyNotFound);
        }

        property.UpdateFrom(command.Request);

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Property {PropertyId} ({Street}, {Number} - {City}) updated successfully",
            property.Id,
            property.Address.Street,
            property.Address.Number,
            property.Address.City);

        return Result.Success();
    }
}