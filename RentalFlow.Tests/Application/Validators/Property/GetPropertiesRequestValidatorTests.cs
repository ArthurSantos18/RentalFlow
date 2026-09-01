using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Validators.Property;

namespace RentalFlow.Tests.Application.Validators.Property;

public sealed class GetPropertiesRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetPropertiesRequestValidator _validator = new();

    [Theory]
    [InlineData(-1, "Minimum rent price cannot be negative.")]
    [InlineData(-100, "Minimum rent price cannot be negative.")]
    public void Validate_MinRentPrice_ShouldHaveError_WhenNegative(decimal minRentPrice, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinRentPrice, minRentPrice)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinRentPrice).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData(-1, "Maximum rent price cannot be negative.")]
    [InlineData(-100, "Maximum rent price cannot be negative.")]
    public void Validate_MaxRentPrice_ShouldHaveError_WhenNegative(decimal maxRentPrice, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MaxRentPrice, maxRentPrice)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxRentPrice).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_MinRentPriceGreaterThanMaxRentPrice_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinRentPrice, 5000)
            .With(r => r.MaxRentPrice, 1000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Minimum rent price cannot be greater than maximum rent price.");
    }

    [Fact]
    public void Validate_MinRentPriceLessThanMaxRentPrice_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinBedrooms, 1)
            .With(r => r.MaxBedrooms, 3)
            .With(r => r.MinRentPrice, 1000m)
            .With(r => r.MaxRentPrice, 5000m)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData(-1, "Minimum number of bedrooms cannot be negative.")]
    [InlineData(-5, "Minimum number of bedrooms cannot be negative.")]
    public void Validate_MinBedrooms_ShouldHaveError_WhenNegative(int minBedrooms, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinBedrooms, minBedrooms)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinBedrooms).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData(-1, "Maximum number of bedrooms cannot be negative.")]
    [InlineData(-5, "Maximum number of bedrooms cannot be negative.")]
    public void Validate_MaxBedrooms_ShouldHaveError_WhenNegative(int maxBedrooms, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MaxBedrooms, maxBedrooms)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxBedrooms).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_MinBedroomsGreaterThanMaxBedrooms_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinBedrooms, 5)
            .With(r => r.MaxBedrooms, 2)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Minimum number of bedrooms cannot be greater than maximum number of bedrooms.");
    }

    [Fact]
    public void Validate_MinBedroomsLessThanMaxBedrooms_ShouldNotHaveError()
    {
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinBedrooms, 1)
            .With(r => r.MaxBedrooms, 3)
            .With(r => r.MinRentPrice, 1000m)
            .With(r => r.MaxRentPrice, 5000m)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData("12345", "Invalid ZIP code format. Use XXXXX-XXX or XXXXXXXX.")]
    [InlineData("12345-67", "Invalid ZIP code format. Use XXXXX-XXX or XXXXXXXX.")]
    [InlineData("ABCDE-123", "Invalid ZIP code format. Use XXXXX-XXX or XXXXXXXX.")]
    [InlineData("1234567", "Invalid ZIP code format. Use XXXXX-XXX or XXXXXXXX.")]
    [InlineData("123456789", "Invalid ZIP code format. Use XXXXX-XXX or XXXXXXXX.")]
    public void Validate_ZipCodes_ShouldHaveError_WhenInvalidFormat(string zipCode, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.ZipCodes, [zipCode])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ZipCodes).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("12345-678")]
    public void Validate_ZipCodes_ShouldNotHaveError_WhenValidFormat(string zipCode)
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.ZipCodes, [zipCode])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ZipCodes);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = _fixture.Build<GetPropertiesRequest>()
            .With(r => r.MinRentPrice, 1000)
            .With(r => r.MaxRentPrice, 5000)
            .With(r => r.MinBedrooms, 2)
            .With(r => r.MaxBedrooms, 4)
            .With(r => r.ZipCodes, ["12345678", "12345-678"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenFieldsAreNull()
    {
        // Arrange
        var request = new GetPropertiesRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}