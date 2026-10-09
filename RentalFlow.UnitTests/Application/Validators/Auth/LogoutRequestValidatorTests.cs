namespace RentalFlow.UnitTests.Application.Validators.Auth;

public sealed class LogoutRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly LogoutRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldHaveError_WhenRefreshTokenIsEmpty()
    {
        var request = _fixture.Build<LogoutRequest>()
            .With(r => r.RefreshToken, string.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRequestIsValid()
    {
        var request = _fixture.Create<LogoutRequest>();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}