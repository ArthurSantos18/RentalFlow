namespace RentalFlow.Application.Validators.Auth;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .Must(PasswordValidator.HasMinimumLength)
            .WithMessage($"New password must be at least {PasswordValidator.MinimumLength} characters.")
            .Must(PasswordValidator.HasUppercase)
            .WithMessage("New password must contain at least one uppercase letter.")
            .Must(PasswordValidator.HasLowercase)
            .WithMessage("New password must contain at least one lowercase letter.")
            .Must(PasswordValidator.HasNumber)
            .WithMessage("New password must contain at least one number.")
            .Must(PasswordValidator.HasSpecialCharacter)
            .WithMessage("New password must contain at least one special character.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}
