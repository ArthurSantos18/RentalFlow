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
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ApplicantId, Guid.Empty)
            .With(r => r.PropertyId, Guid.Empty)
            .With(r => r.OperatorId, Guid.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApplicantId)
            .WithErrorMessage("ApplicantId is required.");

        result.ShouldHaveValidationErrorFor(x => x.PropertyId)
            .WithErrorMessage("PropertyId is required.");

        result.ShouldHaveValidationErrorFor(x => x.OperatorId)
            .WithErrorMessage("OperatorId is required.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAmountsOrInstallmentsInvalid()
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 0m)
            .With(r => r.TotalAmount, 0m)
            .With(r => r.Installments, 0)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FinancedAmount)
            .WithErrorMessage("Financed amount must be greater than zero.");

        result.ShouldHaveValidationErrorFor(x => x.TotalAmount)
            .WithErrorMessage("Total amount must be greater than zero.");

        result.ShouldHaveValidationErrorFor(x => x.Installments)
            .WithErrorMessage("Installments must be greater than zero.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_FinancedAmount_ShouldHaveError(decimal financedAmount)
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.FinancedAmount, financedAmount)
            .With(r => r.TotalAmount, 100m)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FinancedAmount)
            .WithErrorMessage("Financed amount must be greater than zero.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_TotalAmount_ShouldHaveError(decimal totalAmount)
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 100m)
            .With(r => r.TotalAmount, totalAmount)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount)
            .WithErrorMessage("Total amount must be greater than zero.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_Installments_ShouldHaveError(int installments)
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.Installments, installments)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Installments)
            .WithErrorMessage("Installments must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenFinancedGreaterThanTotal()
    {
        // Arrange
        var request = new AddRentalApplicationRequest
        {
            ApplicantId = Guid.NewGuid(),
            PropertyId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
            FinancedAmount = 200m,
            TotalAmount = 100m,
            Installments = 12
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Financed amount must be less than or equal to total amount.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenFinancedLessThanTotal()
    {
        // Arrange
        var request = new AddRentalApplicationRequest
        {
            ApplicantId = Guid.NewGuid(),
            PropertyId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
            FinancedAmount = 100m,
            TotalAmount = 200m,
            Installments = 12
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenFinancedEqualsTotal()
    {
        // Arrange
        var request = new AddRentalApplicationRequest
        {
            ApplicantId = Guid.NewGuid(),
            PropertyId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
            FinancedAmount = 200m,
            TotalAmount = 200m,
            Installments = 12
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenContractDateInFuture()
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ContractDate, DateTime.UtcNow.AddDays(1))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContractDate)
            .WithErrorMessage("Contract date cannot be in the future.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenContractDateIsInThePast()
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ContractDate, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContractDate);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenContractDateIsNull()
    {
        // Arrange
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ContractDate, (DateTime?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ContractDate);
    }

    [Fact]
    public void Validate_ShouldNotHaveAnyErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = new AddRentalApplicationRequest
        {
            ApplicantId = Guid.NewGuid(),
            PropertyId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
            FinancedAmount = 1000m,
            TotalAmount = 1500m,
            Installments = 12,
            ContractDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveMultipleErrors_WhenMultipleFieldsInvalid()
    {
        // Arrange
        var request = new AddRentalApplicationRequest
        {
            ApplicantId = Guid.Empty,
            PropertyId = Guid.Empty,
            OperatorId = Guid.Empty,
            FinancedAmount = 0m,
            TotalAmount = 0m,
            Installments = 0,
            ContractDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApplicantId);
        result.ShouldHaveValidationErrorFor(x => x.PropertyId);
        result.ShouldHaveValidationErrorFor(x => x.OperatorId);
        result.ShouldHaveValidationErrorFor(x => x.FinancedAmount);
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
        result.ShouldHaveValidationErrorFor(x => x.Installments);
        result.ShouldHaveValidationErrorFor(x => x.ContractDate);
    }
}
