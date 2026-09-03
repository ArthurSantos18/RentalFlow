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

    [Fact]
    public void Validate_ShouldHaveError_WhenMinFinancedNegative()
    {
        var request = _fixture.Build<GetRentalApplicationRequest>().With(r => r.MinFinancedAmount, -1m).Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MinFinancedAmount).WithErrorMessage("Minimum financed amount cannot be negative.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMinFinancedGreaterThanMax()
    {
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinFinancedAmount, 200m)
            .With(r => r.MaxFinancedAmount, 100m)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Minimum financed amount cannot be greater than maximum financed amount.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMinTotalGreaterThanMax()
    {
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinTotalAmount, 200m)
            .With(r => r.MaxTotalAmount, 100m)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Minimum total amount cannot be greater than maximum total amount.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMinCreatedAtGreaterThanMax()
    {
        var request = _fixture.Build<GetRentalApplicationRequest>()
            .With(r => r.MinCreatedAt, DateTime.UtcNow.AddDays(1))
            .With(r => r.MaxCreatedAt, DateTime.UtcNow)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Minimum created date cannot be greater than maximum created date.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenProposalNumberEmpty()
    {
        var request = _fixture.Build<GetRentalApplicationRequest>().With(r => r.ProposalNumbers, new[] { string.Empty }).Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ProposalNumbers).WithErrorMessage("Proposal number cannot be empty.");
    }
}
