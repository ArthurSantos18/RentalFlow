namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class AddTeamCommandHandler(ITeamRepository _teamRepository) : ICommandHandler<AddTeamCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddTeamCommand command, CancellationToken cancellationToken)
    {
        var teamNameExist = await _teamRepository.NameExistsAsync(command.Request.Name, cancellationToken);

        if (teamNameExist)
        {
            return Result<Guid>.Failure(TeamErrors.TeamAlreadyExists);
        }

        var newTeam = command.Request.ToEntity();

        await _teamRepository.AddAsync(newTeam, cancellationToken);

        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(newTeam.Id);
    }
}

