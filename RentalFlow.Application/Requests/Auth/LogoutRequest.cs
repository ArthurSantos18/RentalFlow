namespace RentalFlow.Application.Requests.Auth;

public sealed record LogoutRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
