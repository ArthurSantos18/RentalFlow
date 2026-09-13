using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.Validators.Team;

namespace RentalFlow.Tests.Application.Validators.Team;

public sealed class GetTeamsRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetTeamsRequestValidator _validator = new();

    [Fact]
    public void Validate_Ids_ShouldHaveError_WhenContainsEmptyId()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Ids, [Guid.Empty, Guid.NewGuid()])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage("Team ID cannot be empty.");
    }

    [Fact]
    public void Validate_ValidIds_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Ids, [Guid.NewGuid(), Guid.NewGuid()])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Ids);
    }

    [Fact]
    public void Validate_Names_ShouldHaveError_WhenContainsEmptyName()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Names, ["", "Valid Name"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Names)
            .WithErrorMessage("Name cannot be empty.");
    }

    [Fact]
    public void Validate_Names_ShouldHaveError_WhenNameIsTooShort()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Names, ["AB", "Valid Name"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Names)
            .WithErrorMessage("Name must have at least 3 characters.");
    }

    [Fact]
    public void Validate_Names_ShouldHaveError_WhenNameExceedsMaximumLength()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Names, [new string('A', 31)])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Names)
            .WithErrorMessage("Name must not exceed 30 characters.");
    }

    [Fact]
    public void Validate_ValidNames_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Names, ["Team Alpha", "Team Beta"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Names);
    }

    [Fact]
    public void Validate_Description_ShouldHaveError_WhenContainsEmptyDescription()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Description, ["", "Valid Description"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot be empty.");
    }

    [Fact]
    public void Validate_Description_ShouldHaveError_WhenDescriptionIsTooShort()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Description, ["AB", "Valid Description"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must have at least 3 characters.");
    }

    [Fact]
    public void Validate_Description_ShouldHaveError_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Description, [new string('A', 101)])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ValidDescriptions_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Description, ["Valid Description One", "Valid Description Two"])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Ids, [Guid.NewGuid()])
            .With(r => r.Names, ["Team Alpha"])
            .With(r => r.Description, ["Team Description"])
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
        var request = new GetTeamRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = _fixture.Build<GetTeamRequest>()
            .With(r => r.Ids, [Guid.Empty])
            .With(r => r.Names, [""])
            .With(r => r.Description, [""])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids);
        result.ShouldHaveValidationErrorFor(x => x.Names);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
