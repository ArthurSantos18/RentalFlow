using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Validators.Property;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Tests.Application.Validators.Property;

public sealed class UpdatePropertyRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdatePropertyRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenNoFieldsProvided()
    {
        // Arrange
        var request = new UpdatePropertyRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_Street_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Street, new string('A', 101))
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.Street)
            .WithErrorMessage("Street must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_Number_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Number, new string('1', 11))
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.Number)
            .WithErrorMessage("Number must not exceed 10 characters.");
    }

    [Fact]
    public void Validate_Complement_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Complement, new string('A', 51))
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.Complement)
            .WithErrorMessage("Complement must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_Neighborhood_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Neighborhood, new string('A', 51))
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.Neighborhood)
            .WithErrorMessage("Neighborhood must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_City_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.City, new string('A', 51))
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.City)
            .WithErrorMessage("City must not exceed 50 characters.");
    }

    [Theory]
    [InlineData("B")]
    [InlineData("BAA")]
    public void Validate_State_ShouldHaveError_WhenInvalidLength(string state)
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.State, state)
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.State)
            .WithErrorMessage("State must be a 2-letter code (e.g., BA).");
    }

    [Fact]
    public void Validate_State_ShouldHaveError_WhenContainsNonLetters()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.State, "B1")
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.State)
            .WithErrorMessage("State must contain only letters.");
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("ABCDEFGH")]
    [InlineData("12345-67")]
    public void Validate_ZipCode_ShouldHaveError_WhenInvalid(string zipCode)
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.ZipCode, zipCode)
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.ZipCode)
            .WithErrorMessage("ZipCode must be in the format XXXXX-XXX or XXXXXXXX.");
    }

    [Theory]
    [InlineData("01234567")]
    [InlineData("01234-567")]
    public void Validate_ZipCode_ShouldNotHaveError_WhenValid(string zipCode)
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.ZipCode, zipCode)
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Address!.ZipCode);
    }

    [Fact]
    public void Validate_RentPrice_ShouldHaveError_WhenZero()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.RentPrice, 0)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RentPrice)
            .WithErrorMessage("Rent price must be greater than zero.");
    }

    [Fact]
    public void Validate_RentPrice_ShouldHaveError_WhenNegative()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.RentPrice, -100)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RentPrice)
            .WithErrorMessage("Rent price must be greater than zero.");
    }

    [Fact]
    public void Validate_RentPrice_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.RentPrice, 2500m)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RentPrice);
    }

    [Fact]
    public void Validate_Bedrooms_ShouldHaveError_WhenZero()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Bedrooms, 0)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Bedrooms)
            .WithErrorMessage("Bedrooms must be greater than zero.");
    }

    [Fact]
    public void Validate_Bedrooms_ShouldHaveError_WhenNegative()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Bedrooms, -1)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Bedrooms)
            .WithErrorMessage("Bedrooms must be greater than zero.");
    }

    [Fact]
    public void Validate_Bedrooms_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Bedrooms, 2)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Bedrooms);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.Street, "Rua das Flores")
            .With(a => a.Number, "123")
            .With(a => a.Complement, "Apto 101")
            .With(a => a.Neighborhood, "Centro")
            .With(a => a.City, "São Paulo")
            .With(a => a.State, "SP")
            .With(a => a.ZipCode, "01234567")
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, 2500.00m)
            .With(r => r.Bedrooms, 2)
            .With(r => r.IsAvailable, true)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
