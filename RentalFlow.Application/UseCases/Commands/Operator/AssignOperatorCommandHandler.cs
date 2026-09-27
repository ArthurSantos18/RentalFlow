namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AssignOperatorCommandHandler(
    IOperatorRepository _operatorRepository,
    ITeamRepository _teamRepository,
    ICurrentUserService _currentUserService
    ) : ICommandHandler<AssignOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(AssignOperatorCommand command, CancellationToken cancellationToken)
    {
        var @operator = await _operatorRepository.GetByIdWithDetailsAsync(command.OperatorId, cancellationToken);

        if (@operator is null)
        {
            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        var team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);

        if (team is null)
        {
            return Result.Failure(TeamErrors.TeamNotFound);
        }

        if (!team.IsActive)
        {
            return Result.Failure(TeamErrors.TeamInactive);
        }

        if (@operator.TeamId == team.Id)
        {
            return Result.Success();
        }

        var permissionResult = EnsureCanAssign(@operator, team.Id);

        if (permissionResult.IsFailure)
        {
            return permissionResult;
        }

        @operator.SetTeam(team);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private Result EnsureCanAssign(OperatorEntity @operator, Guid teamId)
    {
        if (_currentUserService.Role == nameof(OperatorRole.Administrator))
        {
            return Result.Success();
        }

        if (_currentUserService.Role != nameof(OperatorRole.Manager))
        {
            return Result.Failure(UserErrors.Forbidden);
        }

        if (teamId != _currentUserService.TeamId)
        {
            return Result.Failure(OperatorErrors.CannotMoveToDifferentTeam);
        }

        if (@operator.Role == OperatorRole.Administrator)
        {
            return Result.Failure(OperatorErrors.ManagerCannotManageAdmin);
        }

        return Result.Success();
    }
}