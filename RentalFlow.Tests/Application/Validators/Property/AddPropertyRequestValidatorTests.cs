using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Validators.Property;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Tests.Application.Validators.Property;

public sealed class AddPropertyRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AddPropertyRequestValidator _validator = new();

    [Theory]
    [InlineData("", "Street is required.")]
    [InlineData(null, "Street is required.")]
    public void Validate_Street_ShouldHaveError(string? street, string expectedError)
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Street, street)
            .Create();

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address.Street).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("", "State is required.")]
    [InlineData("B", "State must be a 2-letter code (e.g., BA).")]
    [InlineData("BAA", "State must be a 2-letter code (e.g., BA).")]
    [InlineData("B1", "State must contain only letters.")]
    public void Validate_State_ShouldHaveError(string state, string expectedError)
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.State, state)
            .Create();

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address.State).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("", "ZipCode is required.")]
    [InlineData("1234567", "ZipCode must have exactly 8 digits.")]
    [InlineData("123456789", "ZipCode must have exactly 8 digits.")]
    [InlineData("ABCDEFGH", "ZipCode must be in the format XXXXX-XXX or XXXXXXXX.")]
    [InlineData("12345-67", "ZipCode must be in the format XXXXX-XXX or XXXXXXXX.")]
    public void Validate_ZipCode_ShouldHaveError(string zipCode, string expectedError)
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.ZipCode, zipCode)
            .Create();

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address.ZipCode).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_ValidAddress_ShouldNotHaveError()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Street, "Rua das Flores")
            .With(a => a.Number, "123")
            .With(a => a.Neighborhood, "Centro")
            .With(a => a.City, "São Paulo")
            .With(a => a.State, "SP")
            .With(a => a.ZipCode, "01234567")
            .Create();

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Address.Street);
        result.ShouldNotHaveValidationErrorFor(x => x.Address.Number);
        result.ShouldNotHaveValidationErrorFor(x => x.Address.Neighborhood);
        result.ShouldNotHaveValidationErrorFor(x => x.Address.City);
        result.ShouldNotHaveValidationErrorFor(x => x.Address.State);
        result.ShouldNotHaveValidationErrorFor(x => x.Address.ZipCode);
    }

    [Theory]
    [InlineData(0, "Rent price must be greater than zero.")]
    [InlineData(-100, "Rent price must be greater than zero.")]
    public void Validate_RentPrice_ShouldHaveError(decimal rentPrice, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.RentPrice, rentPrice)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RentPrice).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_ValidRentPrice_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.RentPrice, 2500.00m)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RentPrice);
    }

    [Theory]
    [InlineData(0, "Bedrooms must be greater than zero.")]
    [InlineData(-1, "Bedrooms must be greater than zero.")]
    public void Validate_Bedrooms_ShouldHaveError(int bedrooms, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Bedrooms, bedrooms)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Bedrooms).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_ValidBedrooms_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Bedrooms, 2)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Bedrooms);
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Street, string.Empty)
            .With(a => a.Number, string.Empty)
            .With(a => a.Neighborhood, string.Empty)
            .With(a => a.City, string.Empty)
            .With(a => a.State, "S")
            .With(a => a.ZipCode, "123")
            .Create();

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, 0)
            .With(r => r.Bedrooms, 0)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address.Street);
        result.ShouldHaveValidationErrorFor(x => x.Address.Number);
        result.ShouldHaveValidationErrorFor(x => x.Address.Neighborhood);
        result.ShouldHaveValidationErrorFor(x => x.Address.City);
        result.ShouldHaveValidationErrorFor(x => x.Address.State);
        result.ShouldHaveValidationErrorFor(x => x.Address.ZipCode);
        result.ShouldHaveValidationErrorFor(x => x.RentPrice);
        result.ShouldHaveValidationErrorFor(x => x.Bedrooms);
    }
}