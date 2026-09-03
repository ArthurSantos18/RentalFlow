using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed class ChangeRentalApplicationOperatorCommandHandler(
    IRentalApplicationRepository _rentalRepository,
    IOperatorRepository _operatorRepository
    ) : ICommandHandler<ChangeRentalApplicationOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(ChangeRentalApplicationOperatorCommand command, CancellationToken cancellationToken)
    {
        var rentalApplication = await _rentalRepository.GetByIdAsync(command.Id, cancellationToken);

        if (rentalApplication is null)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationNotFound);
        }

        if (rentalApplication.Status == RentalStatus.Approved || rentalApplication.Status == RentalStatus.Rejected)
        {
            return Result.Failure(RentalApplicationErrors.RentalApplicationOperatorChangeNotAllowed);
        }

        var newOperator = await _operatorRepository.GetByIdAsync(command.Request.NewOperatorId, cancellationToken);

        if (newOperator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }
            
        if (!newOperator.IsActive)
        {
            return Result.Failure(OperatorErrors.OperatorIsInactive);
        }

        if (rentalApplication.OperatorId == newOperator.Id)
        {
            return Result.Failure(RentalApplicationErrors.OperatorAlreadyAssigned);
        }

        rentalApplication.SetOperator(newOperator);

        _rentalRepository.Update(rentalApplication);

        await _rentalRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
