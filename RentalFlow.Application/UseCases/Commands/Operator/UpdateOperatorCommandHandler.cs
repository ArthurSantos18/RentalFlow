using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class UpdateOperatorCommandHandler(IOperatorRepository _operatorRepository) : ICommandHandler<UpdateOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateOperatorCommand command, CancellationToken cancellationToken)
    {
        var @operator = await _operatorRepository.GetByIdAsync(command.Id, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        @operator = command.Request.UpdateEntity(@operator);

        _operatorRepository.Update(@operator);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
