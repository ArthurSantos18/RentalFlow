namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class UpdateTeamCommandHandler(
    ITeamRepository _teamRepository,
    ICurrentUserService _currentUserService
    ) : ICommandHandler<UpdateTeamCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateTeamCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var team = await _teamRepository.GetByIdAsync(command.Id, cancellationToken);

        if (team is null)
        {
            return Result.Failure(TeamErrors.TeamNotFound);
        }

        if (!CanEdit(team))
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != team.Name)
        {
            var nameExists = await _teamRepository.NameExistsAsync(request.Name, cancellationToken);

            if (nameExists)
            {
                return Result.Failure(TeamErrors.TeamAlreadyExists);
            }

        }

        team.UpdateFrom(request);

        await _teamRepository.SaveChangesAsync(cancellationToken);

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