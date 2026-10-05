namespace RentalFlow.Application.Requests.Auth;

public sealed record ChangePasswordRequest
{
    [Description("The current password of the user.")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Description("The new password for the user.")]
    public string NewPassword { get; init; } = string.Empty;

    [Description("The confirmation of the new password.")]
    public string ConfirmNewPassword { get; init; } = string.Empty;
}