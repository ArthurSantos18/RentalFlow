using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Validators.Applicant;

namespace RentalFlow.Tests.Application.Validators.Applicant;

public sealed class AddApplicantRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AddApplicantRequestValidator _validator = new();

    [Theory]
    [InlineData("", "Full name is required.")]
    [InlineData("A", "Full name must have at least 3 characters.")]
    public void Validate_FullName_ShouldHaveError(string fullName, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.FullName, fullName)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_FullNameExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.FullName, new string('A', 101))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ValidFullName_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.FullName, "Arthur Azevedo")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Validate_CpfEmpty_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF is required.");
    }

    [Fact]
    public void Validate_ValidCpf_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Validate_InvalidCpf_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "11111111111")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("Invalid CPF.");
    }

    [Fact]
    public void Validate_EmailEmpty_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Email, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Email, "invalid-email")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Validate_EmailExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Email, $"{new string('a', 92)}@test.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ValidEmail_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Email, "arthur@email.com")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_PhoneEmpty_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Phone, string.Empty)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number is required.");
    }

    [Fact]
    public void Validate_PhoneWithLessThanMinimumLength_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Phone, "1234567")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number must have at least 8 digits.");
    }

    [Fact]
    public void Validate_PhoneExceedingMaximumLength_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Phone, new string('1', 16))
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number must not exceed 15 digits.");
    }

    [Fact]
    public void Validate_ValidPhone_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Phone, "11999999999")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_MonthlyIncomeZero_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.MonthlyIncome, 0)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome)
            .WithErrorMessage("Monthly income must be greater than zero.");
    }

    [Fact]
    public void Validate_MonthlyIncomeNegative_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.MonthlyIncome, -1)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome)
            .WithErrorMessage("Monthly income must be greater than zero.");
    }

    [Fact]
    public void Validate_ValidMonthlyIncome_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.MonthlyIncome, 5000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MonthlyIncome);
    }

    [Fact]
    public void Validate_AllValidFields_ShouldNotHaveError()
    {
        // Arrange
        var request = new AddApplicantRequest
        {
            FullName = "Arthur Azevedo",
            Cpf = "52998224725",
            Email = "arthur@email.com",
            Phone = "11999999999",
            MonthlyIncome = 5000
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_AllFieldsEmpty_ShouldHaveErrors()
    {
        // Arrange
        var request = new AddApplicantRequest
        {
            FullName = string.Empty,
            Cpf = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            MonthlyIncome = 0
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName);
        result.ShouldHaveValidationErrorFor(x => x.Cpf);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Phone);
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome);
    }
}
