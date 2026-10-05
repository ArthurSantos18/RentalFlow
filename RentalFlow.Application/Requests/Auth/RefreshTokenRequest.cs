namespace RentalFlow.Application.Requests.Auth;

public sealed record RefreshTokenRequest
{
    [Description("O token de atualização do usuário.")]
    public string RefreshToken { get; init; } = string.Empty;
}