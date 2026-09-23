namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandler(IOperatorRepository _operatorRepository,
    ITeamRepository _teamRepository,
    IUserRepository _userRepository,
    IPasswordService _passwordService) : ICommandHandler<AddOperatorCommand, Result<AddOperatorResponse>>
{
    public async Task<Result<AddOperatorResponse>> HandleAsync(AddOperatorCommand command, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(command.Request.TeamId, cancellationToken);

        if (team is null)
        {
            return Result<AddOperatorResponse>.Failure(TeamErrors.TeamNotFound);
        }

        if (!team.IsActive)
        {
            return Result<AddOperatorResponse>.Failure(TeamErrors.TeamInactive);
        }

        var emailExists = await _userRepository.EmailExistsAsync(command.Request.Email, cancellationToken);
        
        if (emailExists)
        {
            return Result<AddOperatorResponse>.Failure(UserErrors.EmailAlreadyExists);
        }

        var @operator = command.Request.ToEntity(team);

        await _operatorRepository.AddAsync(@operator, cancellationToken);

        var temporaryPassword = _passwordService.GenerateTemporaryPassword();

        var passwordHash = _passwordService.Hash(temporaryPassword);

        var user = command.Request.ToEntity(passwordHash, @operator);

        await _userRepository.AddAsync(user, cancellationToken);

        await _userRepository.SaveChangesAsync(cancellationToken);

        var response = @operator.ToResponse(user, temporaryPassword);

        return Result<AddOperatorResponse>.Success(response);
    }
}
