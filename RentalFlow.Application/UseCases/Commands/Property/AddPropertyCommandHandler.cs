namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed class AddPropertyCommandHandler(
    IPropertyRepository _propertyRepository,
    ILogger<AddPropertyCommandHandler> _logger
    ) : ICommandHandler<AddPropertyCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddPropertyCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Creating property at {Street}, {Number} - {City}/{State} with rent {RentPrice}",
            request.Address.Street,
            request.Address.Number,
            request.Address.City,
            request.Address.State,
            request.RentPrice);

        var newProperty = request.ToEntity();

        await _propertyRepository.AddAsync(newProperty, cancellationToken);

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Property {PropertyId} ({Street}, {Number} - {City}) created successfully",
            newProperty.Id,
            newProperty.Address.Street,
            newProperty.Address.Number,
            newProperty.Address.City);

        return Result<Guid>.Success(newProperty.Id);
    }
}