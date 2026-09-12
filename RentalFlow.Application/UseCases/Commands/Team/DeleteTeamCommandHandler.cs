using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class DeleteTeamCommandHandler(ITeamRepository _teamRepository) : ICommandHandler<DeleteTeamCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(command.Id, cancellationToken);

        if (team is null)
        {
            return Result.Failure(TeamErrors.TeamNotFound);
        }

        if (team.IsActive == false)
        {
            return Result.Success();
        }

        team.SetIsActive(false);

        _teamRepository.Update(team);

        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
