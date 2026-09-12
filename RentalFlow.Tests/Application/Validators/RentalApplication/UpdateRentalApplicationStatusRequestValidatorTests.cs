using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Validators.RentalApplication;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Tests.Application.Validators.RentalApplication;

public sealed class UpdateRentalApplicationStatusRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateRentalApplicationStatusRequestValidator _validator = new();

    [Theory]
    [InlineData(RentalStatus.Draft)]
    [InlineData(RentalStatus.Pending)]
    [InlineData(RentalStatus.Approved)]
    [InlineData(RentalStatus.Rejected)]
    public void Validate_ShouldNotHaveError_WhenRentalStatusIsValid(RentalStatus status)
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationStatusRequest>()
            .With(r => r.RentalStatus, status)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RentalStatus);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(999)]
    public void Validate_ShouldHaveError_WhenRentalStatusIsInvalid(int status)
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationStatusRequest>()
            .With(r => r.RentalStatus, (RentalStatus)status)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RentalStatus)
            .WithErrorMessage("Rental status must be a valid status.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRentalStatusIsDefault()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationStatusRequest>()
            .With(r => r.RentalStatus, RentalStatus.Draft)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RentalStatus);
    }
}
