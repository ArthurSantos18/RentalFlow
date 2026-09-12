using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Validators.RentalApplication;

namespace RentalFlow.Tests.Application.Validators.RentalApplication;

public sealed class UpdateRentalApplicationRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateRentalApplicationRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenNoFieldsProvided()
    {
        // Arrange
        var request = new UpdateRentalApplicationRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_Installments_ShouldHaveError_WhenInvalid(int installments)
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.Installments, installments)
            .Without(r => r.FinancedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Installments)
            .WithErrorMessage("Installments must be greater than zero.");
    }

    [Fact]
    public void Validate_Installments_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.Installments, 12)
            .Without(r => r.FinancedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Installments);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Validate_FinancedAmount_ShouldHaveError_WhenInvalid(decimal financedAmount)
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, financedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FinancedAmount)
            .WithErrorMessage("Financed amount must be greater than zero.");
    }

    [Fact]
    public void Validate_FinancedAmount_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 1000m)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FinancedAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Validate_TotalAmount_ShouldHaveError_WhenInvalid(decimal totalAmount)
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.TotalAmount, totalAmount)
            .Without(r => r.FinancedAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount)
            .WithErrorMessage("Total amount must be greater than zero.");
    }

    [Fact]
    public void Validate_TotalAmount_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.TotalAmount, 2000m)
            .Without(r => r.FinancedAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TotalAmount);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenFinancedGreaterThanTotal()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 200m)
            .With(r => r.TotalAmount, 100m)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Financed amount must be less than or equal to total amount.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenFinancedEqualsTotal()
    {
        // Arrange
        var amount = _fixture.Create<decimal>();

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, amount)
            .With(r => r.TotalAmount, amount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenFinancedLessThanTotal()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 100m)
            .With(r => r.TotalAmount, 200m)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenOnlyFinancedAmountIsProvided()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 100m)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenOnlyTotalAmountIsProvided()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.TotalAmount, 100m)
            .Without(r => r.FinancedAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ContractDate_ShouldHaveError_WhenInFuture()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ContractDate, DateTime.UtcNow.AddDays(1))
            .Without(r => r.FinancedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContractDate)
            .WithErrorMessage("Contract date cannot be in the future.");
    }

    [Fact]
    public void Validate_ContractDate_ShouldNotHaveError_WhenInPast()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ContractDate, DateTime.UtcNow.AddMinutes(-1))
            .Without(r => r.FinancedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContractDate);
    }

    [Fact]
    public void Validate_ContractDate_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ContractDate)
            .Without(r => r.FinancedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContractDate);
    }
}
