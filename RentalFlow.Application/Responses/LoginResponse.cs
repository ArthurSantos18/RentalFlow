namespace RentalFlow.Application.Responses;

public sealed record LoginResponse
{
    [Description("The access token for the authenticated user.")]
    public string AccessToken { get; init; } = string.Empty;

    [Description("The refresh token for the authenticated user.")]
    public string RefreshToken { get; init; } = string.Empty;

    [Description("Indicates whether the user must change their password.")]
    public bool MustChangePassword { get; init; }

    [Description("The unique identifier of the authenticated user.")]
    public Guid UserId { get; init; }

    [Description("The email address of the authenticated user.")]
    public string Email { get; init; } = string.Empty;

    [Description("The role of the authenticated user.")]
    public string Role { get; init; } = string.Empty;
}