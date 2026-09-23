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
}
