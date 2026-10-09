namespace RentalFlow.UnitTests.Application.Validators.Report;

public sealed class GetApplicationsByPeriodRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetApplicationsByPeriodRequestValidator _validator = new();

    [Fact]
    public void Validate_From_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.From, default(DateTime))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.From)
            .WithErrorMessage("The start date is required.");
    }

    [Fact]
    public void Validate_From_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.From, DateTime.UtcNow)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.From);
    }

    [Fact]
    public void Validate_To_ShouldHaveError_WhenEmpty()
    {
        // Arrange
        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.From, DateTime.UtcNow)
            .With(r => r.To, default(DateTime))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.To)
            .WithErrorMessage("The end date is required.");
    }

    [Fact]
    public void Validate_To_ShouldHaveError_WhenLessThanFrom()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(-1);

        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
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

        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
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

        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.From, date)
            .With(r => r.To, date)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(999)]
    public void Validate_GroupBy_ShouldHaveError_WhenInvalid(int groupBy)
    {
        // Arrange
        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.GroupBy, (PeriodGroup)groupBy)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.GroupBy)
            .WithErrorMessage("The group by is invalid.");
    }

    [Theory]
    [InlineData(PeriodGroup.Day)]
    [InlineData(PeriodGroup.Week)]
    [InlineData(PeriodGroup.Month)]
    public void Validate_GroupBy_ShouldNotHaveError_WhenValid(PeriodGroup groupBy)
    {
        // Arrange
        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.GroupBy, groupBy)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.GroupBy);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var from = DateTime.UtcNow;
        var to = from.AddDays(1);

        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.From, from)
            .With(r => r.To, to)
            .With(r => r.GroupBy, PeriodGroup.Day)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
        // Arrange
        var request = new GetApplicationsByPeriodRequest
        {
            From = default,
            To = default,
            GroupBy = (PeriodGroup)(-1)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.From)
            .WithErrorMessage("The start date is required.");

        result.ShouldHaveValidationErrorFor(x => x.To)
            .WithErrorMessage("The end date is required.");

        result.ShouldHaveValidationErrorFor(x => x.GroupBy)
            .WithErrorMessage("The group by is invalid.");
    }
}