namespace RentalFlow.Application.Requests.Auth;

public sealed record LogoutRequest
{
    [Description("O token de atualização do usuário.")]
    public string RefreshToken { get; init; } = string.Empty;
}