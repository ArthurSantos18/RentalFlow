namespace RentalFlow.Application.UseCases.Queries.User;

public sealed class GetCurrentUserQueryHandler(
    IUserRepository _userRepository,
    ICurrentUserService _currentUserService,
    ILogger<GetCurrentUserQueryHandler> _logger) : IQueryHandler<GetCurrentUserQuery, Result<GetCurrentUserResponse>>
{
    public async Task<Result<GetCurrentUserResponse>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning(
                "User not found. UserId: {UserId}",
                userId);

            return Result<GetCurrentUserResponse>.Failure(UserErrors.UserNotFound);
        }

        return Result<GetCurrentUserResponse>.Success(user.ToResponse());
    }
}