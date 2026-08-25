using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests;
using RentalFlow.Application.Validators;

namespace RentalFlow.Tests.Application.Validators;

public sealed class UpdateApplicantRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateApplicantRequestValidator _validator = new();

    [Fact]
    public void Validate_FullName_ShouldHaveError_WhenTooShort()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>().With(r => r.FullName, "Ab").Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName).WithErrorMessage("Full name must have at least 3 characters.");
    }

    [Fact]
    public void Validate_Cpf_ShouldHaveError_WhenInvalid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>().With(r => r.Cpf, "11111111111").Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf).WithErrorMessage("Invalid CPF.");
    }

    [Fact]
    public void Validate_Email_ShouldHaveError_WhenInvalidFormat()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>().With(r => r.Email, "invalid").Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Validate_Phone_ShouldHaveError_WhenTooShort()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>().With(r => r.Phone, "1234567").Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone).WithErrorMessage("Phone number must have at least 8 digits.");
    }

    [Fact]
    public void Validate_MonthlyIncome_ShouldHaveError_WhenZero()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>().With(r => r.MonthlyIncome, 0).Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome).WithErrorMessage("Monthly income must be greater than zero.");
    }
}
