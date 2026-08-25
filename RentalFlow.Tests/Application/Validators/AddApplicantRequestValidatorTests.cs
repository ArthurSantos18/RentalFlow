using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests;
using RentalFlow.Application.Validators;

namespace RentalFlow.Tests.Application.Validators;

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
        var request = _fixture.Build<AddApplicantRequest>().With(r => r.FullName, fullName).Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName).WithErrorMessage(expectedError);
    }

    [Fact]
    public void Validate_ValidCpf_ShouldNotHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>().With(r => r.Cpf, "52998224725").Create();
        
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Validate_InvalidCpf_ShouldHaveError()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>().With(r => r.Cpf, "11111111111").Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf).WithErrorMessage("Invalid CPF.");
    }
}
