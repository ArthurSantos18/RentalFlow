using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Validators.Operator;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Tests.Application.Validators.Operator;

public sealed class AddOperatorRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AddOperatorRequestValidator _validator = new();

    [Theory]
    [InlineData("", "Name is required.")]
    [InlineData(null, "Name is required.")]
    public void Validate_Name_ShouldHaveError(string? name, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Name, name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_NameExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var name = new string('A', 101);

        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Name, name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ValidName_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Name, "Beatrice Umineko")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("", "Email is required.")]
    [InlineData(null, "Email is required.")]
    public void Validate_Email_ShouldHaveError(string? email, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Email, email)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Email, "invalid-email")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email must be a valid email address.");
    }

    [Fact]
    public void Validate_EmailExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var email = $"{new string('a', 92)}@test.com";

        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Email, email)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ValidEmail_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Email, "beatrice@test.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_EmptyTeamId_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.TeamId, Guid.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TeamId).WithErrorMessage("TeamId is required.");
    }

    [Fact]
    public void Validate_ValidTeamId_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.TeamId, Guid.NewGuid())
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TeamId);
    }

    [Fact]
    public void Validate_RoleNone_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Role, OperatorRole.None)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role).WithErrorMessage("Role is required.");
    }

    [Theory]
    [InlineData(OperatorRole.Broker)]
    [InlineData(OperatorRole.Manager)]
    [InlineData(OperatorRole.Administrator)]
    public void Validate_ValidRole_ShouldNotHaveError(OperatorRole role)
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Role, role)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Name, string.Empty)
            .With(r => r.Email, string.Empty)
            .With(r => r.Role, OperatorRole.None)
            .With(r => r.TeamId, Guid.NewGuid())
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Role);
        result.ShouldNotHaveValidationErrorFor(x => x.TeamId);
    }
}