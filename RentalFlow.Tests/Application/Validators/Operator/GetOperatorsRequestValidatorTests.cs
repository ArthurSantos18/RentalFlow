using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Validators.Operator;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Tests.Application.Validators.Operator;

public sealed class GetOperatorsRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetOperatorsRequestValidator _validator = new();

    [Fact]
    public void Validate_Ids_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Ids, [Guid.Empty])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("Operator ID cannot be empty.");
    }

    [Fact]
    public void Validate_Ids_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Ids, [Guid.NewGuid()])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Ids);
    }

    [Theory]
    [InlineData("")]
    public void Validate_Names_ShouldHaveError_WhenEmpty(string name)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Names, [name])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Names)
            .WithErrorMessage("Name cannot be empty.");
    }

    [Theory]
    [InlineData("Jo", "Name must have at least 3 characters.")]
    [InlineData("A", "Name must have at least 3 characters.")]
    public void Validate_Names_ShouldHaveError_WhenBelowMinimumLength(
        string name,
        string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Names, [name])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Names)
            .WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_Names_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var name = new string('A', 101);

        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Names, [name])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Names)
            .WithErrorMessage("Name must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("John")]
    [InlineData("John Doe")]
    public void Validate_Names_ShouldNotHaveError_WhenValid(string name)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Names, [name])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Names);
    }

    [Theory]
    [InlineData("")]
    public void Validate_Emails_ShouldHaveError_WhenEmpty(string email)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Emails)
            .WithErrorMessage("Email cannot be empty.");
    }

    [Theory]
    [InlineData("invalid-email", "Invalid email format.")]
    [InlineData("invalid@", "Invalid email format.")]
    [InlineData("@email.com", "Invalid email format.")]
    public void Validate_Emails_ShouldHaveError_WhenInvalidFormat(
        string email,
        string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Emails)
            .WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_Emails_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var email = $"{new string('a', 90)}@example.com";

        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Emails)
            .WithErrorMessage("Email must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("test@example.com")]
    public void Validate_Emails_ShouldNotHaveError_WhenValidFormat(string email)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Emails);
    }

    [Fact]
    public void Validate_Role_ShouldHaveError_WhenInvalid()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Role, (OperatorRole)999)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role)
            .WithErrorMessage("Invalid operator role.");
    }

    [Theory]
    [InlineData(OperatorRole.None)]
    [InlineData(OperatorRole.Broker)]
    [InlineData(OperatorRole.Manager)]
    [InlineData(OperatorRole.Administrator)]
    public void Validate_Role_ShouldNotHaveError_WhenValid(OperatorRole role)
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Role, role)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public void Validate_ApplicationIds_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.ApplicationIds, [Guid.Empty])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApplicationIds)
            .WithErrorMessage("Application ID cannot be empty.");
    }

    [Fact]
    public void Validate_ApplicationIds_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.ApplicationIds, [Guid.NewGuid()])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ApplicationIds);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Ids, [Guid.NewGuid()])
            .With(r => r.Names, ["John Doe"])
            .With(r => r.Emails, ["john.doe@example.com"])
            .With(r => r.Role, OperatorRole.Broker)
            .With(r => r.ApplicationIds, [Guid.NewGuid()])
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
        var request = new GetOperatorRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = _fixture.Build<GetOperatorRequest>()
            .With(r => r.Ids, [Guid.Empty])
            .With(r => r.Names, ["Jo"])
            .With(r => r.Emails, ["invalid-email"])
            .With(r => r.Role, (OperatorRole)999)
            .With(r => r.ApplicationIds, [Guid.Empty])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids);
        result.ShouldHaveValidationErrorFor(x => x.Names);
        result.ShouldHaveValidationErrorFor(x => x.Emails);
        result.ShouldHaveValidationErrorFor(x => x.Role);
        result.ShouldHaveValidationErrorFor(x => x.ApplicationIds);
    }
}
