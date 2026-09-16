namespace RentalFlow.Tests.Application.Validators.Team;

public sealed class AddTeamRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AddTeamRequestValidator _validator = new();

    [Theory]
    [InlineData("", "Name is required.")]
    [InlineData(null, "Name is required.")]
    public void Validate_Name_ShouldHaveError(string? name, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_NameWithLessThanMinimumLength_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, "AB")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must have at least 3 characters.");
    }

    [Fact]
    public void Validate_NameExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var name = new string('A', 31);

        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, name)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("Name must not exceed 30 characters.");
    }

    [Fact]
    public void Validate_ValidName_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, "Team Alpha")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("", "Description is required.")]
    [InlineData(null, "Description is required.")]
    public void Validate_Description_ShouldHaveError(string? description, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Description, description)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_DescriptionWithLessThanMinimumLength_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Description, "AB")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).WithErrorMessage("Description must have at least 3 characters.");
    }

    [Fact]
    public void Validate_DescriptionExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var description = new string('A', 101);

        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Description, description)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).WithErrorMessage("Description must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ValidDescription_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Description, "Team Alpha Description")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, string.Empty)
            .With(r => r.Description, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
