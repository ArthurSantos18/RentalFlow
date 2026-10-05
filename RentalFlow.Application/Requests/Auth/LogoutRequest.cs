namespace RentalFlow.Application.Requests.Auth;

public sealed record LogoutRequest
{
    [Description("The refresh token of the user.")]
    public string RefreshToken { get; init; } = string.Empty;
}