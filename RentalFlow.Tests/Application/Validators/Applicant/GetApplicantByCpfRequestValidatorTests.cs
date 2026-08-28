using AutoFixture;
using FluentValidation.TestHelper;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Validators.Applicant;

namespace RentalFlow.Tests.Application.Validators.Applicant;

public sealed class GetApplicantByCpfRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetApplicantByCpfRequestValidator _validator = new();

    [Theory]
    [InlineData("", "CPF is required for search.")]
    [InlineData("11111111111", "Invalid CPF.")]
    [InlineData("123", "Invalid CPF.")]
    public void Validate_Cpf_ShouldHaveError(string cpf, string expectedMessage)
    {
        // Arrange
        var request = _fixture.Build<GetApplicantByCpfRequest>()
            .With(r => r.Cpf, cpf)
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf).WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void Validate_ValidCpf_ShouldNotHaveError()
    {
        // Arrnge
        var request = _fixture.Build<GetApplicantByCpfRequest>()
            .With(r => r.Cpf, "52998224725")
            .Create();

        // Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

}
