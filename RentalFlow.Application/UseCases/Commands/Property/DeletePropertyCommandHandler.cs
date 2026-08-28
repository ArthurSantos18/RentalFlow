using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

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

        if (property.IsActive == false)
        {
            return Result.Success();
        }

        property
            .SetIsActive(false);

        _propertyRepository.Update(property);

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
