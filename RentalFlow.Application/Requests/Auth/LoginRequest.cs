namespace RentalFlow.Application.Requests.Auth;

public sealed record LoginRequest
{
    [Description("O email do usuário.")]
    public string Email { get; init; } = string.Empty;

    [Description("A senha do usuário.")]
    public string Password { get; init; } = string.Empty;
}