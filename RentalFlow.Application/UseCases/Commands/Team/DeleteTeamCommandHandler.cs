namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class DeleteTeamCommandHandler(
    ITeamRepository _teamRepository,
    IOperatorRepository _operatorRepository,
    ILogger<DeleteTeamCommandHandler> _logger
    ) : ICommandHandler<DeleteTeamCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteTeamCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting team {TeamId}",
            command.Id);

        var team = await _teamRepository.GetByIdAsync(command.Id, cancellationToken);

        if (team is null)
        {
            _logger.LogWarning(
                "Team {TeamId} not found for deletion: {ErrorCode} {ErrorMessage}",
                command.Id,
                TeamErrors.TeamNotFound.Code,
                TeamErrors.TeamNotFound.Message);

            return Result.Failure(TeamErrors.TeamNotFound);
        }

        var activeOperatorsCount = await _operatorRepository.CountActiveByTeamAsync(team.Id, cancellationToken);

        if (activeOperatorsCount > 0)
        {
            _logger.LogWarning(
                "Cannot delete team {TeamId} ({TeamName}) because it has {ActiveOperatorsCount} active operators: {ErrorCode} {ErrorMessage}",
                team.Id,
                team.Name,
                activeOperatorsCount,
                TeamErrors.TeamHasActiveOperators.Code,
                TeamErrors.TeamHasActiveOperators.Message);

            return Result.Failure(TeamErrors.TeamHasActiveOperators);
        }

        team.MarkAsDeleted();

        await _teamRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Team {TeamId} ({TeamName}) deleted successfully",
            team.Id,
            team.Name);

        return Result.Success();
    }
}