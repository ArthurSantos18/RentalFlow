namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class UpdateTeamCommandHandler(
    ITeamRepository _teamRepository,
    ICurrentUserService _currentUserService,
    ILogger<UpdateTeamCommandHandler> _logger
    ) : ICommandHandler<UpdateTeamCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateTeamCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Updating team {TeamId} with name {TeamName}",
            command.Id,
            request.Name);

        var team = await _teamRepository.GetByIdAsync(command.Id, cancellationToken);

        if (team is null)
        {
            _logger.LogWarning(
                "Team {TeamId} not found for update: {ErrorCode} {ErrorMessage}",
                command.Id,
                TeamErrors.TeamNotFound.Code,
                TeamErrors.TeamNotFound.Message);

            return Result.Failure(TeamErrors.TeamNotFound);
        }

        if (!CanEdit(team))
        {
            _logger.LogWarning(
                "Forbidden to update team {TeamId}: {ErrorCode} {ErrorMessage}",
                team.Id,
                UserErrors.InvalidRole.Code,
                UserErrors.InvalidRole.Message);

            return Result.Failure(UserErrors.InvalidRole);
        }

        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != team.Name)
        {
            var nameExists = await _teamRepository.NameExistsAsync(request.Name, cancellationToken);

            if (nameExists)
            {
                _logger.LogWarning(
                    "Cannot rename team {TeamId} to {TeamName} because that name already exists: {ErrorCode} {ErrorMessage}",
                    team.Id,
                    request.Name,
                    TeamErrors.TeamAlreadyExists.Code,
                    TeamErrors.TeamAlreadyExists.Message);

                return Result.Failure(TeamErrors.TeamAlreadyExists);
            }
        }

        var previousName = team.Name;

        team.UpdateFrom(request);

        await _teamRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Team {TeamId} renamed from {PreviousName} to {NewName} successfully",
            team.Id,
            previousName,
            team.Name);

        return Result.Success();
    }

    private bool CanEdit(TeamEntity team)
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Administrator) => true,

            nameof(OperatorRole.Manager) => team.Id == _currentUserService.TeamId,

            _ => false
        };
    }
}