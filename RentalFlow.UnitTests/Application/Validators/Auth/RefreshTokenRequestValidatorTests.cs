namespace RentalFlow.UnitTests.Application.Validators.Auth;

public sealed class RefreshTokenRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly RefreshTokenRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldHaveError_WhenRefreshTokenIsEmpty()
    {
        var request = _fixture.Build<RefreshTokenRequest>()
            .With(r => r.RefreshToken, string.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRequestIsValid()
    {
        var request = _fixture.Create<RefreshTokenRequest>();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}