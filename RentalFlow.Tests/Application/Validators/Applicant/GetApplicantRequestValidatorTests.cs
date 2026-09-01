using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Validators.Applicant;

namespace RentalFlow.Tests.Application.Validators.Applicant;

public sealed class GetApplicantRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetApplicantRequestValidator _validator = new();

    [Theory]
    [InlineData("", "CPF cannot be empty.")]
    public void Validate_Cpfs_ShouldHaveError_WhenEmpty(string cpf, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Cpfs, [cpf])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpfs).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("12345678900", "Invalid CPF.")]
    [InlineData("11111111111", "Invalid CPF.")]
    public void Validate_Cpfs_ShouldHaveError_WhenInvalid(string cpf, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Cpfs, [cpf])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpfs).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("52998224725")]
    public void Validate_Cpfs_ShouldNotHaveError_WhenValid(string cpf)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Cpfs, [cpf])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpfs);
    }

    [Theory]
    [InlineData("", "Email cannot be empty.")]
    public void Validate_Emails_ShouldHaveError_WhenEmpty(string email, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Emails).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("invalid-email", "Invalid email format.")]
    [InlineData("invalid@", "Invalid email format.")]
    [InlineData("@email.com", "Invalid email format.")]
    public void Validate_Emails_ShouldHaveError_WhenInvalidFormat(string email, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Emails).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_Emails_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var email = $"{new string('a', 90)}@example.com";

        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Emails)
            .WithErrorMessage("Email must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("test@example.com")]
    public void Validate_Emails_ShouldNotHaveError_WhenValidFormat(string email)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Emails, [email])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Emails);
    }

    [Theory]
    [InlineData("", "Full name cannot be empty.")]
    public void Validate_FullNames_ShouldHaveError_WhenEmpty(string fullName, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.FullNames, [fullName])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullNames).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("Jo", "Full name must have at least 3 characters.")]
    [InlineData("A", "Full name must have at least 3 characters.")]
    public void Validate_FullNames_ShouldHaveError_WhenBelowMinimumLength(
        string fullName,
        string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.FullNames, [fullName])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullNames).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_FullNames_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var fullName = new string('A', 101);

        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.FullNames, [fullName])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullNames)
            .WithErrorMessage("Full name must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("John")]
    [InlineData("John Doe")]
    public void Validate_FullNames_ShouldNotHaveError_WhenValid(string fullName)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.FullNames, [fullName])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FullNames);
    }

    [Theory]
    [InlineData("", "Phone number cannot be empty.")]
    public void Validate_Phones_ShouldHaveError_WhenEmpty(string phone, string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Phones, [phone])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phones).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("1234567", "Phone number must have at least 8 digits.")]
    [InlineData("123456", "Phone number must have at least 8 digits.")]
    public void Validate_Phones_ShouldHaveError_WhenBelowMinimumLength(
        string phone,
        string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Phones, [phone])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phones).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_Phones_ShouldHaveError_WhenExceedsMaximumLength()
    {
        // Arrange
        var phone = new string('1', 16);

        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Phones, [phone])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phones)
            .WithErrorMessage("Phone number must not exceed 15 digits.");
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("11987654321")]
    public void Validate_Phones_ShouldNotHaveError_WhenValid(string phone)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Phones, [phone])
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Phones);
    }

    [Theory]
    [InlineData(-1, "Minimum monthly income cannot be negative.")]
    [InlineData(-1000, "Minimum monthly income cannot be negative.")]
    public void Validate_MinMonthlyIncome_ShouldHaveError_WhenNegative(
        decimal minMonthlyIncome,
        string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.MinMonthlyIncome, minMonthlyIncome)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinMonthlyIncome).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData(-1, "Maximum monthly income cannot be negative.")]
    [InlineData(-1000, "Maximum monthly income cannot be negative.")]
    public void Validate_MaxMonthlyIncome_ShouldHaveError_WhenNegative(
        decimal maxMonthlyIncome,
        string expectedError)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.MaxMonthlyIncome, maxMonthlyIncome)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MaxMonthlyIncome).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_MinMonthlyIncomeGreaterThanMaxMonthlyIncome_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.MinMonthlyIncome, 5000)
            .With(r => r.MaxMonthlyIncome, 1000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Minimum monthly income must be less than or equal to maximum monthly income.");
    }

    [Fact]
    public void Validate_MinMonthlyIncomeLessThanMaxMonthlyIncome_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.MinMonthlyIncome, 1000)
            .With(r => r.MaxMonthlyIncome, 5000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_MinMonthlyIncomeEqualToMaxMonthlyIncome_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.MinMonthlyIncome, 3000)
            .With(r => r.MaxMonthlyIncome, 3000)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllFieldsAreValid()
    {
        // Arrange
        var request = _fixture.Build<GetApplicantRequest>()
            .With(r => r.Cpfs, ["52998224725"])
            .With(r => r.Emails, ["john.doe@example.com"])
            .With(r => r.FullNames, ["John Doe"])
            .With(r => r.Phones, ["11987654321"])
            .With(r => r.MinMonthlyIncome, 1000)
            .With(r => r.MaxMonthlyIncome, 5000)
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
        var request = new GetApplicantRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}