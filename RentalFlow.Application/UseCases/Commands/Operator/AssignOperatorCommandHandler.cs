namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AssignOperatorCommandHandler(
    IOperatorRepository _operatorRepository,
    ITeamRepository _teamRepository,
    ICurrentUserService _currentUserService,
    ILogger<AssignOperatorCommandHandler> _logger
    ) : ICommandHandler<AssignOperatorCommand, Result>
{
    public async Task<Result> HandleAsync(AssignOperatorCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Assigning operator {OperatorId} to team {TeamId}",
            command.OperatorId,
            command.TeamId);

        var @operator = await _operatorRepository.GetByIdWithDetailsAsync(command.OperatorId, cancellationToken);

        if (@operator is null)
        {
            _logger.LogWarning(
                "Operator {OperatorId} not found for assignment: {ErrorCode} {ErrorMessage}",
                command.OperatorId,
                OperatorErrors.OperatorNotFound.Code,
                OperatorErrors.OperatorNotFound.Message);

            return Result.Failure(OperatorErrors.OperatorNotFound);
        }

        var team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);

        if (team is null)
        {
            _logger.LogWarning(
                "Team {TeamId} not found for operator assignment: {ErrorCode} {ErrorMessage}",
                command.TeamId,
                TeamErrors.TeamNotFound.Code,
                TeamErrors.TeamNotFound.Message);

            return Result.Failure(TeamErrors.TeamNotFound);
        }

        if (!team.IsActive)
        {
            _logger.LogWarning(
                "Cannot assign operator {OperatorId} to inactive team {TeamId}: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                team.Id,
                TeamErrors.TeamInactive.Code,
                TeamErrors.TeamInactive.Message);

            return Result.Failure(TeamErrors.TeamInactive);
        }

        if (@operator.TeamId == team.Id)
        {
            _logger.LogInformation(
                "Operator {OperatorId} is already assigned to team {TeamId}, no changes needed",
                @operator.Id,
                team.Id);

            return Result.Success();
        }

        var permissionResult = EnsureCanAssign(@operator, team.Id);

        if (permissionResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot assign operator {OperatorId} to team {TeamId}: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                team.Id,
                permissionResult.Error.Code,
                permissionResult.Error.Message);

            return permissionResult;
        }

        @operator.SetTeam(team);

        await _operatorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Operator {OperatorId} assigned to team {TeamId} successfully",
            @operator.Id,
            team.Id);

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