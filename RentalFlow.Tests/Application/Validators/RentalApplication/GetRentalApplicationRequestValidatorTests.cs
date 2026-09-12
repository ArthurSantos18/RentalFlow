using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Validators.RentalApplication;

namespace RentalFlow.Tests.Application.Validators.RentalApplication;

public sealed class GetRentalApplicationRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetRentalApplicationRequestValidator _validator = new();

    [Theory]
    [InlineData(-1, "Minimum financed amount cannot be negative.")]
    [InlineData(-100, "Minimum financed amount cannot be negative.")]
    public void Validate_MinFinancedAmount_ShouldHaveError_WhenNegative(decimal minFinancedAmount, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, minFinancedAmount)
            .With(r => r.MaxFinancedAmount, 1000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinFinancedAmount)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData(-1, "Maximum financed amount cannot be negative.")]
    [InlineData(-100, "Maximum financed amount cannot be negative.")]
    public void Validate_MaxFinancedAmount_ShouldHaveError_WhenNegative(decimal maxFinancedAmount, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 0)
            .With(r => r.MaxFinancedAmount, maxFinancedAmount)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxFinancedAmount)
            .WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_MinFinancedAmountGreaterThanMaxFinancedAmount_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 200)
            .With(r => r.MaxFinancedAmount, 100)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum financed amount cannot be greater than maximum financed amount.");
    }

    [Fact]
    public void Validate_MinFinancedAmountLessThanMaxFinancedAmount_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 100)
            .With(r => r.MaxFinancedAmount, 200)
            .With(r => r.MinTotalAmount, 100)
            .With(r => r.MaxTotalAmount, 200)
            .With(r => r.MinCreatedAt, DateTime.UtcNow)
            .With(r => r.MaxCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MinContractDate, DateTime.UtcNow)
            .With(r => r.MaxContractDate, DateTime.UtcNow.AddDays(1))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData(-1, "Minimum total amount cannot be negative.")]
    [InlineData(-100, "Minimum total amount cannot be negative.")]
    public void Validate_MinTotalAmount_ShouldHaveError_WhenNegative(decimal minTotalAmount, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinTotalAmount, minTotalAmount)
            .With(r => r.MaxTotalAmount, 1000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinTotalAmount)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData(-1, "Maximum total amount cannot be negative.")]
    [InlineData(-100, "Maximum total amount cannot be negative.")]
    public void Validate_MaxTotalAmount_ShouldHaveError_WhenNegative(decimal maxTotalAmount, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinTotalAmount, 0)
            .With(r => r.MaxTotalAmount, maxTotalAmount)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxTotalAmount)
            .WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_MinTotalAmountGreaterThanMaxTotalAmount_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinTotalAmount, 200)
            .With(r => r.MaxTotalAmount, 100)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum total amount cannot be greater than maximum total amount.");
    }

    [Fact]
    public void Validate_MinTotalAmountLessThanMaxTotalAmount_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 100)
            .With(r => r.MaxFinancedAmount, 200)
            .With(r => r.MinTotalAmount, 100)
            .With(r => r.MaxTotalAmount, 200)
            .With(r => r.MinCreatedAt, DateTime.UtcNow)
            .With(r => r.MaxCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MinContractDate, DateTime.UtcNow)
            .With(r => r.MaxContractDate, DateTime.UtcNow.AddDays(1))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_MinCreatedAtGreaterThanMaxCreatedAt_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MaxCreatedAt, DateTime.UtcNow)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum created date cannot be greater than maximum created date.");
    }

    [Fact]
    public void Validate_MinCreatedAtLessThanMaxCreatedAt_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 100)
            .With(r => r.MaxFinancedAmount, 200)
            .With(r => r.MinTotalAmount, 100)
            .With(r => r.MaxTotalAmount, 200)
            .With(r => r.MinCreatedAt, DateTime.UtcNow)
            .With(r => r.MaxCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MinContractDate, DateTime.UtcNow)
            .With(r => r.MaxContractDate, DateTime.UtcNow.AddDays(1))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_MinContractDateGreaterThanMaxContractDate_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinContractDate, DateTime.UtcNow.AddDays(1))
            .With(r => r.MaxContractDate, DateTime.UtcNow)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum contract date cannot be greater than maximum contract date.");
    }

    [Fact]
    public void Validate_MinContractDateLessThanMaxContractDate_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 100)
            .With(r => r.MaxFinancedAmount, 200)
            .With(r => r.MinTotalAmount, 100)
            .With(r => r.MaxTotalAmount, 200)
            .With(r => r.MinCreatedAt, DateTime.UtcNow)
            .With(r => r.MaxCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MinContractDate, DateTime.UtcNow)
            .With(r => r.MaxContractDate, DateTime.UtcNow.AddDays(1))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData("")]
    public void Validate_ProposalNumbers_ShouldHaveError_WhenEmpty(string proposalNumber)
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.ProposalNumbers, [proposalNumber])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProposalNumbers)
            .WithErrorMessage("Proposal number cannot be empty.");
    }

    [Fact]
    public void Validate_ProposalNumbers_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var proposalNumber = new string('A', 51);

        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.ProposalNumbers, [proposalNumber])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProposalNumbers)
            .WithErrorMessage("Proposal number must not exceed 50 characters.");
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("PROP-001")]
    public void Validate_ProposalNumbers_ShouldNotHaveError_WhenValid(string proposalNumber)
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.ProposalNumbers, [proposalNumber])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProposalNumbers);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 1000)
            .With(r => r.MaxFinancedAmount, 5000)
            .With(r => r.MinTotalAmount, 5000)
            .With(r => r.MaxTotalAmount, 10000)
            .With(r => r.MinCreatedAt, DateTime.UtcNow)
            .With(r => r.MaxCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MinContractDate, DateTime.UtcNow)
            .With(r => r.MaxContractDate, DateTime.UtcNow.AddDays(1))
            .With(r => r.ProposalNumbers, ["PROP-001"])
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
        var request = new GetRentalApplicationRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, -1)
            .With(r => r.MaxFinancedAmount, -100)
            .With(r => r.MinTotalAmount, -1)
            .With(r => r.MaxTotalAmount, -100)
            .With(r => r.MinCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MaxCreatedAt, DateTime.UtcNow)
            .With(r => r.MinContractDate, DateTime.UtcNow.AddDays(1))
            .With(r => r.MaxContractDate, DateTime.UtcNow)
            .With(r => r.ProposalNumbers, [string.Empty])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinFinancedAmount);
        result.ShouldHaveValidationErrorFor(x => x.MaxFinancedAmount);
        result.ShouldHaveValidationErrorFor(x => x.MinTotalAmount);
        result.ShouldHaveValidationErrorFor(x => x.MaxTotalAmount);
        result.ShouldHaveValidationErrorFor(x => x.ProposalNumbers);
    }
}
