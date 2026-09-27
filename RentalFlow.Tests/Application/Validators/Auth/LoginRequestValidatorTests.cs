namespace RentalFlow.Tests.Application.Validators.Auth;

public sealed class LoginRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty()
    {
        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, string.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailHasInvalidFormat()
    {
        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, _fixture.Create<string>())
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
    {
        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Password, string.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRequestIsValid()
    {
        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, _fixture.Create<MailAddress>().Address)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenRequestIsInvalid()
    {
        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, string.Empty)
            .With(r => r.Password, string.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }
}