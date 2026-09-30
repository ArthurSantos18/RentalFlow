namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class DeleteTeamCommandHandler(
    ITeamRepository _teamRepository,
    IOperatorRepository _operatorRepository
    ) : ICommandHandler<DeleteTeamCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(command.Id, cancellationToken);

        if (team is null)
        {
            return Result.Failure(TeamErrors.TeamNotFound);
        }

        var activeOperatorsCount = await _operatorRepository.CountActiveByTeamAsync(team.Id, cancellationToken);

        if (activeOperatorsCount > 0)
        {
            return Result.Failure(TeamErrors.TeamHasActiveOperators);
        }

        team.MarkAsDeleted();

        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}