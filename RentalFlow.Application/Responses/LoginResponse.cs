namespace RentalFlow.Application.Responses;

public sealed record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public bool MustChangePassword { get; init; }
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
