using FluentValidation;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Helpers;

namespace RentalFlow.Application.Validators.Applicant;

public sealed class GetApplicantRequestValidator : AbstractValidator<GetApplicantRequest>
{
    public GetApplicantRequestValidator()
    {
        RuleForEach(x => x.Cpfs)
           .NotEmpty()
           .WithMessage("CPF cannot be empty.")
           .Must(CpfValidator.IsValid)
           .WithMessage("Invalid CPF.")
           .When(x => x.Cpfs?.Any() == true);

        RuleForEach(x => x.Emails)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.")
            .When(x => x.Emails?.Any() == true);

        RuleForEach(x => x.FullNames)
            .NotEmpty()
            .WithMessage("Full name cannot be empty.")
            .MinimumLength(3)
            .WithMessage("Full name must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters.")
            .When(x => x.FullNames?.Any() == true);

        RuleForEach(x => x.Phones)
            .NotEmpty()
            .WithMessage("Phone number cannot be empty.")
            .MinimumLength(8)
            .WithMessage("Phone number must have at least 8 digits.")
            .MaximumLength(15)
            .WithMessage("Phone number must not exceed 15 digits.")
            .When(x => x.Phones?.Any() == true);

        RuleFor(x => x.MinMonthlyIncome)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum monthly income cannot be negative.")
            .When(x => x.MinMonthlyIncome.HasValue);

        RuleFor(x => x.MaxMonthlyIncome)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maximum monthly income cannot be negative.")
            .When(x => x.MaxMonthlyIncome.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinMonthlyIncome.HasValue || !x.MaxMonthlyIncome.HasValue || x.MinMonthlyIncome <= x.MaxMonthlyIncome)
            .WithMessage("Minimum monthly income must be less than or equal to maximum monthly income.");
    }
}
