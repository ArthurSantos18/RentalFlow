namespace RentalFlow.Application.Requests.Auth;

public sealed record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
