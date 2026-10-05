namespace RentalFlow.Application.Requests.Auth;

public sealed record ChangePasswordRequest
{
    [Description("A senha atual do usuário.")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Description("A nova senha para o usuário.")]
    public string NewPassword { get; init; } = string.Empty;

    [Description("A confirmação da nova senha.")]
    public string ConfirmNewPassword { get; init; } = string.Empty;
}