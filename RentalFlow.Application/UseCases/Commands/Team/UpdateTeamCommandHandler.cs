using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class UpdateTeamCommandHandler(ITeamRepository _teamRepository) : ICommandHandler<UpdateTeamCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(command.Id, cancellationToken);

        if (team is null)
        {
            return Result.Failure(TeamErrors.TeamNotFound);
        }

        team = command.Request.UpdateEntity(team);

        _teamRepository.Update(team);

        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
