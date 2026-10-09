namespace RentalFlow.UnitTests.Application.Validators.Report;

public sealed class GetTopOperatorsRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetTopOperatorsRequestValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(51)]
    [InlineData(100)]
    public void Validate_Limit_ShouldHaveError_WhenOutsideAllowedRange(int limit)
    {
        // Arrange
        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.Limit, limit)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorMessage("The limit must be between 1 and 50.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public void Validate_Limit_ShouldNotHaveError_WhenWithinAllowedRange(int limit)
    {
        // Arrange
        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.Limit, limit)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Validate_To_ShouldHaveError_WhenLessThanFrom()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(-1);

        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.From, from)
            .With(r => r.To, to)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.To)
            .WithErrorMessage("The end date must be greater than or equal to the start date.");
    }

    [Fact]
    public void Validate_To_ShouldNotHaveError_WhenGreaterThanFrom()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(1);

        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.From, from)
            .With(r => r.To, to)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void Validate_To_ShouldNotHaveError_WhenEqualToFrom()
    {
        // Arrange
        var date = DateTime.UtcNow;

        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.From, date)
            .With(r => r.To, date)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void Validate_To_ShouldNotHaveError_WhenFromIsNull()
    {
        // Arrange
        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.From, (DateTime?)null)
            .With(r => r.To, DateTime.UtcNow)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void Validate_To_ShouldNotHaveError_WhenToIsNull()
    {
        // Arrange
        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.From, DateTime.UtcNow)
            .With(r => r.To, (DateTime?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenFieldsAreNull()
    {
        // Arrange
        var request = new GetTopOperatorsRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(1);

        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.Limit, 10)
            .With(r => r.From, from)
            .With(r => r.To, to)
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
        var from = DateTime.UtcNow;
        var to = from.AddDays(-1);

        var request = _fixture.Build<GetTopOperatorsRequest>()
            .With(r => r.Limit, 0)
            .With(r => r.From, from)
            .With(r => r.To, to)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorMessage("The limit must be between 1 and 50.");

        result.ShouldHaveValidationErrorFor(x => x.To)
            .WithErrorMessage("The end date must be greater than or equal to the start date.");
    }
}