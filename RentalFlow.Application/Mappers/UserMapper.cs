namespace RentalFlow.Application.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(this AddOperatorRequest request, string passwordHash, OperatorEntity @operator)
    {
        return new UserEntity(
            request.Email,
            passwordHash,
            true,
            @operator);
    }

    public static GetCurrentUserResponse ToResponse(this UserEntity entity)
    {
        return new GetCurrentUserResponse
        {
            Email = entity.Email,
            MustChangePassword = entity.MustChangePassword,
            OperatorId = entity.Operator?.Id,
            OperatorName = entity.Operator?.Name,
            OperatorRole = entity.Operator?.Role.ToString(),
            TeamId = entity.Operator?.Team?.Id,
            TeamName = entity.Operator?.Team?.Name
        };
    }
}