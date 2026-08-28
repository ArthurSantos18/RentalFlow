using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed class UpdatePropertyCommandHandler(IPropertyRepository _propertyRepository) : ICommandHandler<UpdatePropertyCommand, Result>
{
    public async Task<Result> HandleAsync(UpdatePropertyCommand command, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(command.Id, cancellationToken);

        if (property is null)
        {
            return Result.Failure(PropertyErrors.PropertyNotFound);
        }

        var update = command.Request.ToUpdateDomain();

        property.Update(update);

        _propertyRepository.Update(property);

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
