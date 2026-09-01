using FluentValidation;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Helpers;

namespace RentalFlow.Application.Validators.Applicant;

public sealed class UpdateApplicantRequestValidator : AbstractValidator<UpdateApplicantRequest>
{
    public UpdateApplicantRequestValidator()
    {
        RuleFor(x => x.FullName)
            .MinimumLength(3)
            .WithMessage("Full name must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.")
            .When(x => x.FullName is not null);

        RuleFor(x => x.Cpf)
            .Must(CpfValidator.IsValid)
            .WithMessage("Invalid CPF.")
            .When(x => x.Cpf is not null);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.")
            .When(x => x.Email is not null);

        RuleFor(x => x.Phone)
            .MinimumLength(8)
            .WithMessage("Phone number must have at least 8 digits.")
            .MaximumLength(15)
            .WithMessage("Phone number must not exceed 15 digits.")
            .When(x => x.Phone is not null);

        RuleFor(x => x.MonthlyIncome)
            .GreaterThan(0)
            .WithMessage("Monthly income must be greater than zero.")
            .When(x => x.MonthlyIncome.HasValue);
    }
}
