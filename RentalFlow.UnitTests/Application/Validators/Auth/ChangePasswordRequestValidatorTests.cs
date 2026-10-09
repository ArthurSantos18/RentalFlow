namespace RentalFlow.UnitTests.Application.Validators.Auth;

public sealed class ChangePasswordRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly ChangePasswordRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldHaveError_WhenCurrentPasswordIsEmpty()
    {
        // Arrange
        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.CurrentPassword, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword)
            .WithErrorMessage("Current password is required.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNewPasswordIsEmpty()
    {
        // Arrange
        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password is required.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNewPasswordIsTooShort()
    {
        // Arrange
        var password = _fixture.Create<string>()[..(PasswordValidator.MinimumLength - 1)];

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage($"New password must be at least {PasswordValidator.MinimumLength} characters.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNewPasswordHasNoUppercase()
    {
        // Arrange
        var password = CreateValidPassword()
            .ToLowerInvariant();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNewPasswordHasNoLowercase()
    {
        // Arrange
        var password = CreateValidPassword()
            .ToUpperInvariant();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNewPasswordHasNoNumber()
    {
        // Arrange
        var password = $"{CreateRandomLetters()}!";

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one number.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNewPasswordHasNoSpecialCharacter()
    {
        // Arrange
        var password = $"{CreateRandomLetters()}{_fixture.Create<int>()}";

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one special character.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var newPassword = CreateValidPassword();
        var confirmPassword = CreateValidPassword();

        while (confirmPassword == newPassword)
        {
            confirmPassword = CreateValidPassword();
        }

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, newPassword)
            .With(r => r.ConfirmNewPassword, confirmPassword)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword)
            .WithErrorMessage("Passwords do not match.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenPasswordsMatch()
    {
        // Arrange
        var password = CreateValidPassword();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .With(r => r.ConfirmNewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ConfirmNewPassword);
    }

    [Fact]
    public void Validate_ShouldNotHaveAnyValidationErrors_WhenRequestIsValid()
    {
        // Arrange
        var password = CreateValidPassword();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, password)
            .With(r => r.ConfirmNewPassword, password)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenNewPasswordIsInvalid()
    {
        // Arrange
        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.NewPassword, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password is required.");
    }

    private string CreateValidPassword()
    {
        var uppercase = _fixture.Create<string>()
            .Where(char.IsLetter)
            .Select(char.ToUpperInvariant)
            .First();

        var lowercase = _fixture.Create<string>()
            .Where(char.IsLetter)
            .Select(char.ToLowerInvariant)
            .First();

        var number = Math.Abs(_fixture.Create<int>()) % 10;

        var specialCharacters = "!@#$%^&*";
        var specialCharacter = specialCharacters[
            _fixture.Create<int>() % specialCharacters.Length];

        var remainingLength = Math.Max(
            PasswordValidator.MinimumLength - 4,
            _fixture.Create<int>() % 8);

        var remainingCharacters = _fixture.Create<string>()
            .Where(char.IsLetterOrDigit)
            .Take(remainingLength)
            .ToArray();

        return $"{uppercase}{lowercase}{number}{specialCharacter}{new string(remainingCharacters)}";
    }

    private string CreateRandomLetters()
    {
        var length = PasswordValidator.MinimumLength + Math.Abs(_fixture.Create<int>() % 8);

        var letters = _fixture.CreateMany<char>()
            .Where(char.IsLetter)
            .Take(length)
            .ToArray();

        return new string(letters);
    }
}