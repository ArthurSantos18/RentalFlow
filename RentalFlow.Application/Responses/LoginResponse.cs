namespace RentalFlow.Application.Responses;

public sealed record LoginResponse
{
    [Description("O token de acesso do usuário autenticado.")]
    public string AccessToken { get; init; } = string.Empty;

    [Description("O token de atualização do usuário autenticado.")]
    public string RefreshToken { get; init; } = string.Empty;

    [Description("Indica se o usuário deve alterar sua senha.")]
    public bool MustChangePassword { get; init; }

    [Description("O identificador único do usuário autenticado.")]
    public Guid UserId { get; init; }

    [Description("O endereço de email do usuário autenticado.")]
    public string Email { get; init; } = string.Empty;

    [Description("A função do usuário autenticado.")]
    public string Role { get; init; } = string.Empty;
}