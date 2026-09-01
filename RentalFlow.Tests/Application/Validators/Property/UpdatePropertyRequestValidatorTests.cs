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
    public void Validate_State_ShouldHaveError_WhenInvalid()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.State, "B")
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.State).WithErrorMessage("State must be a 2-letter code (e.g., BA).");
    }

    [Fact]
    public void Validate_ZipCode_ShouldHaveError_WhenInvalid()
    {
        // Arrange
        var address = _fixture.Build<Address>()
            .With(a => a.ZipCode, "123")
            .Create();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address!.ZipCode).WithErrorMessage("ZipCode must have exactly 8 digits.");
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
        result.ShouldHaveValidationErrorFor(x => x.RentPrice).WithErrorMessage("Rent price must be greater than zero.");
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
        result.ShouldHaveValidationErrorFor(x => x.Bedrooms).WithErrorMessage("Bedrooms must be greater than zero.");
    }

    [Fact]
    public void Validate_IsAvailable_ShouldHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdatePropertyRequest>()
            .Without(r => r.IsAvailable)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsAvailable);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
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
