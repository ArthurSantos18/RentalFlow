using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class DeleteOperatorCommandHandler(IOperatorRepository _operatorRepository) : ICommandHandler<DeleteOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteOperatorCommand command, CancellationToken cancellationToken = default)
    {
        var @operator = await _operatorRepository.GetByIdAsync(command.Id, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        if (@operator.IsActive == false)
        {
            return Result.Success();
        }

        @operator.SetIsActive(false);

        _operatorRepository.Update(@operator);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
