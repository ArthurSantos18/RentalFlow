using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Validators.RentalApplication;

namespace RentalFlow.Tests.Application.Validators.RentalApplication;

public sealed class AddRentalApplicationRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AddRentalApplicationRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsEmpty()
    {
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ApplicantId, Guid.Empty)
            .With(r => r.PropertyId, Guid.Empty)
            .With(r => r.OperatorId, Guid.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ApplicantId);
        result.ShouldHaveValidationErrorFor(x => x.PropertyId);
        result.ShouldHaveValidationErrorFor(x => x.OperatorId);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountsOrInstallmentsInvalid()
    {
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 0m)
            .With(r => r.TotalAmount, 0m)
            .With(r => r.Installments, 0)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FinancedAmount);
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
        result.ShouldHaveValidationErrorFor(x => x.Installments);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenFinancedGreaterThanTotal()
    {
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 200m)
            .With(r => r.TotalAmount, 100m)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x).WithErrorMessage("Financed amount must be less than or equal to total amount.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenContractDateInFuture()
    {
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ContractDate, DateTime.UtcNow.AddDays(1))
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContractDate);
    }
}
