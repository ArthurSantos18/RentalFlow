using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.Validators.Team;

namespace RentalFlow.Tests.Application.Validators.Team;

public sealed class UpdateTeamRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateTeamRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenNoFieldsProvided()
    {
        // Arrange
        var request = new UpdateTeamRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void Validate_Name_ShouldHaveError_WhenTooShort(string name)
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, name)
            .Without(r => r.Description)
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
        var name = new string('A', 31);

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, name)
            .Without(r => r.Description)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must not exceed 30 characters.");
    }

    [Fact]
    public void Validate_Name_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, "Team Alpha")
            .Without(r => r.Description)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void Validate_Description_ShouldHaveError_WhenTooShort(string description)
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Description, description)
            .Without(r => r.Name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must have at least 3 characters.");
    }

    [Fact]
    public void Validate_Description_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var description = new string('A', 101);

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Description, description)
            .Without(r => r.Name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_Description_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Description, "Team Description")
            .Without(r => r.Name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenBothFieldsAreValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, "Team Alpha")
            .With(r => r.Description, "Team Description")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, "AB")
            .With(r => r.Description, "AB")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
