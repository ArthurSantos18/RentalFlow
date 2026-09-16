namespace RentalFlow.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(UserEntity user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromExpiredToken(string token);
    DateTime GetRefreshTokenExpiration();
}
