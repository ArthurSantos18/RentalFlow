namespace RentalFlow.Application.UseCases.Queries.User;

public sealed class GetCurrentUserQueryHandler(IUserRepository _userRepository) : IQueryHandler<GetCurrentUserQuery, Result<GetCurrentUserResponse>>
{
    public async Task<Result<GetCurrentUserResponse>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return Result<GetCurrentUserResponse>.Failure(UserErrors.UserNotFound);
        }

        return Result<GetCurrentUserResponse>.Success(user.ToResponse());
    }
}