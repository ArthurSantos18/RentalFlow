using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Validators.Applicant;

namespace RentalFlow.Tests.Application.Validators.Applicant;

public sealed class UpdateApplicantRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly UpdateApplicantRequestValidator _validator = new();

    [Fact]
    public void Validate_FullName_ShouldHaveError_WhenTooShort()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, "Ab")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must have at least 3 characters.");
    }

    [Fact]
    public void Validate_FullName_ShouldHaveError_WhenTooLong()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, new string('A', 101))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_FullName_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, "Arthur Azevedo")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Validate_FullName_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, (string?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Validate_Cpf_ShouldHaveError_WhenInvalid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Cpf, "11111111111")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("Invalid CPF.");
    }

    [Fact]
    public void Validate_Cpf_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Validate_Cpf_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Cpf, (string?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Validate_Email_ShouldHaveError_WhenInvalidFormat()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Email, "invalid")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Validate_Email_ShouldHaveError_WhenTooLong()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Email, $"{new string('a', 92)}@test.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_Email_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Email, "arthur@email.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_Email_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Email, (string?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_Phone_ShouldHaveError_WhenTooShort()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Phone, "1234567")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number must have at least 8 digits.");
    }

    [Fact]
    public void Validate_Phone_ShouldHaveError_WhenTooLong()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Phone, new string('1', 16))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number must not exceed 15 digits.");
    }

    [Fact]
    public void Validate_Phone_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Phone, "71999999999")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_Phone_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Phone, (string?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_MonthlyIncome_ShouldHaveError_WhenZero()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.MonthlyIncome, 0)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome)
            .WithErrorMessage("Monthly income must be greater than zero.");
    }

    [Fact]
    public void Validate_MonthlyIncome_ShouldHaveError_WhenNegative()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.MonthlyIncome, -100)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome)
            .WithErrorMessage("Monthly income must be greater than zero.");
    }

    [Fact]
    public void Validate_MonthlyIncome_ShouldNotHaveError_WhenValid()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.MonthlyIncome, 3500)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MonthlyIncome);
    }

    [Fact]
    public void Validate_MonthlyIncome_ShouldNotHaveError_WhenNull()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.MonthlyIncome, (decimal?)null)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MonthlyIncome);
    }
}
