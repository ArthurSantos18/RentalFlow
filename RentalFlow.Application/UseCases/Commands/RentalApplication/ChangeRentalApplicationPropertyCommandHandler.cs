using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class ChangeRentalApplicationPropertyCommandHandler(
    IRentalApplicationRepository _rentalRepository,
    IPropertyRepository _propertyRepository
) : ICommandHandler<ChangeRentalApplicationPropertyCommand, Result>
{
    public async Task<Result> HandleAsync(ChangeRentalApplicationPropertyCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalRepository.GetByIdAsync(command.Id, cancellationToken);
        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (rentalApplication.Status != RentalStatus.Draft && rentalApplication.Status != RentalStatus.Pending)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationPropertyChangeNotAllowed);
        }    

        var newProperty = await _propertyRepository.GetByIdAsync(command.Request.NewPropertyId, cancellationToken);

        if (newProperty is null)
        {
            return Result.Failure(PropertyErrors.PropertyNotFound);
        }
            
        if (!newProperty.IsActive)
        {
            return Result.Failure(PropertyErrors.PropertyIsInactive);
        }
            
        if (!newProperty.IsAvailable)
        {
            return Result.Failure(PropertyErrors.PropertyNotAvailable);
        }

        if (rentalApplication.PropertyId == newProperty.Id)
        {
            return Result.Failure(RentalApplicationErrors.PropertyAlreadyAssigned);
        }

        rentalApplication.SetProperty(newProperty);

        _rentalRepository.Update(rentalApplication);

        await _rentalRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
