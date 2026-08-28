using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Validators.Property;

namespace RentalFlow.Tests.Application.Validators.Property;

public sealed class GetPropertyByIdRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetPropertyByIdRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenIdIsValid()
    {
        // Arrange
        var request = _fixture.Build<GetPropertyByIdRequest>()
            .With(r => r.Id, Guid.NewGuid())
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdIsEmpty()
    {
        // Arrange
        var request = _fixture.Build<GetPropertyByIdRequest>()
            .With(r => r.Id, Guid.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Property ID is required.");
    }
}