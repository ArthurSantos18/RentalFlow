namespace RentalFlow.Application.Requests.Auth;

public sealed record LoginRequest
{
    [Description("The email address of the user.")]
    public string Email { get; init; } = string.Empty;

    [Description("The password for the user.")]
    public string Password { get; init; } = string.Empty;
}