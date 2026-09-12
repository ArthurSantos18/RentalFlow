using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed class AddTeamCommandHandler(ITeamRepository _teamRepository) : ICommandHandler<AddTeamCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddTeamCommand command, CancellationToken cancellationToken)
    {
        var team = _teamRepository.FindAsync(t => t.Name == command.Request.Name, cancellationToken);

        if (team is not null)
        {
            return Result<Guid>.Failure(TeamErrors.TeamDoesExist);
        }

        var newTeam = command.Request.ToEntity();

        await _teamRepository.AddAsync(newTeam, cancellationToken);

        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(newTeam.Id);
    }
}

