using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Validators.Operator;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Tests.Application.Validators.Operator;

public sealed class UpdateOperatorRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateOperatorRequestValidator _validator = new();

    [Fact]
    public void Validate_Name_ShouldHaveError_WhenTooShort()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, "Ab")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must have at least 3 characters.");
    }

    [Fact]
    public void Validate_Name_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, new string('A', 101))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_Name_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, "Arthur Azevedo")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_Name_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, (string?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_Email_ShouldHaveError_WhenInvalidFormat()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Email, "invalid")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address.");
    }

    [Fact]
    public void Validate_Email_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Email, $"{new string('a', 90)}@example.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_Email_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Email, "arthur@example.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_Email_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Email, (string?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_Role_ShouldHaveError_WhenNone()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Role, OperatorRole.None)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("Role is required.");
    }

    [Theory]
    [InlineData(OperatorRole.Broker)]
    [InlineData(OperatorRole.Manager)]
    [InlineData(OperatorRole.Administrator)]
    public void Validate_Role_ShouldNotHaveError_WhenValid(OperatorRole role)
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Role, role)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validate_Role_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Role, (OperatorRole?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRequestIsEmpty()
    {
        // Arrange
        var request = new UpdateOperatorRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
