using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed class AddPropertyCommandHandler(IPropertyRepository _propertyRepository) : ICommandHandler<AddPropertyCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddPropertyCommand command, CancellationToken cancellationToken)
    {
        var newProperty = command.Request.ToEntity();

        await _propertyRepository.AddAsync(newProperty, cancellationToken);

        await _propertyRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(newProperty.Id);
    }
}
