using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandler(IOperatorRepository _operatorRepository, ITeamRepository _teamRepository) : ICommandHandler<AddOperatorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddOperatorCommand command, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(command.Request.TeamId, cancellationToken);

        if (team is null)
        {
            return Result<Guid>.Failure(TeamErrors.TeamNotFound);
        }

        var @operator = command.Request.ToEntity(team);

        await _operatorRepository.AddAsync(@operator, cancellationToken);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(@operator.Id);
    }
}
