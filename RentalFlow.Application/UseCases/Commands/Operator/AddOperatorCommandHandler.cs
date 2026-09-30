namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandler(
    IOperatorRepository _operatorRepository,
    ITeamRepository _teamRepository,
    IUserRepository _userRepository,
    IPasswordService _passwordService,
    ICurrentUserService _currentUserService,
    ILogger<AddOperatorCommandHandler> _logger
    ) : ICommandHandler<AddOperatorCommand, Result<AddOperatorResponse>>
{
    public async Task<Result<AddOperatorResponse>> HandleAsync(AddOperatorCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Creating operator with email {Email}, role {Role} for team {TeamId}",
            request.Email,
            request.Role,
            request.TeamId);

        var roleValidation = EnsureCanCreateRole(request.Role);

        if (roleValidation.IsFailure)
        {
            _logger.LogWarning(
                "Cannot create operator with role {Role}: {ErrorCode} {ErrorMessage}",
                request.Role,
                roleValidation.Error.Code,
                roleValidation.Error.Message);

            return Result<AddOperatorResponse>.Failure(roleValidation.Error);
        }

        var teamResult = await ResolveTeamAsync(request.TeamId, cancellationToken);

        if (teamResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot create operator for team {TeamId}: {ErrorCode} {ErrorMessage}",
                request.TeamId,
                teamResult.Error.Code,
                teamResult.Error.Message);

            return Result<AddOperatorResponse>.Failure(teamResult.Error);
        }

        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            _logger.LogWarning(
                "Cannot create operator because email {Email} already exists: {ErrorCode} {ErrorMessage}",
                request.Email,
                UserErrors.EmailAlreadyExists.Code,
                UserErrors.EmailAlreadyExists.Message);

            return Result<AddOperatorResponse>.Failure(UserErrors.EmailAlreadyExists);
        }

        var @operator = request.ToEntity(teamResult.Value);

        await _operatorRepository.AddAsync(@operator, cancellationToken);

        var temporaryPassword = _passwordService.GenerateTemporaryPassword();

        var passwordHash = _passwordService.Hash(temporaryPassword);

        var user = request.ToEntity(passwordHash, @operator);

        await _userRepository.AddAsync(user, cancellationToken);

        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Operator {OperatorId} created successfully with email {Email} for team {TeamId}",
            @operator.Id,
            user.Email,
            teamResult.Value.Id);

        var response = @operator.ToResponse(user, temporaryPassword);

        return Result<AddOperatorResponse>.Success(response);
    }

    private Result EnsureCanCreateRole(OperatorRole role)
    {
        if (role == OperatorRole.None)
        {
            return Result.Failure(UserErrors.InvalidRole);
        }

        if (_currentUserService.Role == nameof(OperatorRole.Manager) && role == OperatorRole.Administrator)
        {
            return Result.Failure(OperatorErrors.ManagerCannotManageAdmin);
        }

        return Result.Success();
    }

    private async Task<Result<TeamEntity>> ResolveTeamAsync(Guid teamId, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);

        if (team is null)
        {
            return Result<TeamEntity>.Failure(TeamErrors.TeamNotFound);
        }

        if (!team.IsActive)
        {
            return Result<TeamEntity>.Failure(TeamErrors.TeamInactive);
        }

        if (_currentUserService.Role == nameof(OperatorRole.Manager) && team.Id != _currentUserService.TeamId)
        {
            return Result<TeamEntity>.Failure(OperatorErrors.OperatorNotInTeam);
        }

        return Result<TeamEntity>.Success(team);
    }
}