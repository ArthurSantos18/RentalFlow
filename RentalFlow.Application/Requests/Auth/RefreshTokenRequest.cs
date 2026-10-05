namespace RentalFlow.Application.Requests.Auth;

public sealed record RefreshTokenRequest
{
    [Description("The refresh token of the user.")]
    public string RefreshToken { get; init; } = string.Empty;
}