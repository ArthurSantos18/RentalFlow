namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed class DeletePropertyCommandHandler(IPropertyRepository _propertyRepository) : ICommandHandler<DeletePropertyCommand, Result>
{
    public async Task<Result> HandleAsync(DeletePropertyCommand command, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(command.Id, cancellationToken);

        if (property is null)
        {
            return Result.Failure(PropertyErrors.PropertyNotFound);
        }

        property.MarkAsDeleted();

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
