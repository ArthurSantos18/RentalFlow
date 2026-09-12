using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AssignOperatorCommandHandler(ITeamRepository _teamRepository, IOperatorRepository _operatorRepository) : ICommandHandler<AssignOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(AssignOperatorCommand command, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);

        if (team is null)
        {
            return Result.Failure(TeamErrors.TeamNotFound);
        }

        var @operator = await _operatorRepository.GetByIdAsync(command.OperatorId, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        if (@operator.TeamId == team.Id)
        {
            return Result.Failure(OperatorErrors.OperatorAlreadyAssigned);
        }

        @operator.SetTeam(team);

        _operatorRepository.Update(@operator);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
