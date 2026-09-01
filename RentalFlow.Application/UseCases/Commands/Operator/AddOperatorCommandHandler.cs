using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandler(IOperatorRepository _operatorRepository) : ICommandHandler<AddOperatorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddOperatorCommand command, CancellationToken cancellationToken)
    {
        var newOperator = command.Request.ToEntity();

        await _operatorRepository.AddAsync(newOperator, cancellationToken);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(newOperator.Id);
    }
}
