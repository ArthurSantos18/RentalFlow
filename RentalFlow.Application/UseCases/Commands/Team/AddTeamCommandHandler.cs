namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class AddTeamCommandHandler(
    ITeamRepository _teamRepository,
    ILogger<AddTeamCommandHandler> _logger
    ) : ICommandHandler<AddTeamCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddTeamCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Creating team with name {TeamName}",
            request.Name);

        var teamNameExist = await _teamRepository.NameExistsAsync(request.Name, cancellationToken);

        if (teamNameExist)
        {
            _logger.LogWarning(
                "Cannot create team because name {TeamName} already exists: {ErrorCode} {ErrorMessage}",
                request.Name,
                TeamErrors.TeamAlreadyExists.Code,
                TeamErrors.TeamAlreadyExists.Message);

            return Result<Guid>.Failure(TeamErrors.TeamAlreadyExists);
        }

        var newTeam = request.ToEntity();

        await _teamRepository.AddAsync(newTeam, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Team {TeamId} ({TeamName}) created successfully",
            newTeam.Id,
            newTeam.Name);

        return Result<Guid>.Success(newTeam.Id);
    }
}