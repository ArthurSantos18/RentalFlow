using FluentValidation;
using RentalFlow.Application.Requests;
using RentalFlow.Domain.Helpers;

namespace RentalFlow.Application.Validators;

public sealed class AddApplicantRequestValidator : AbstractValidator<AddApplicantRequest>
{
    public AddApplicantRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(3).WithMessage("Full name must have at least 3 characters.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required.")
            .Length(11).WithMessage("CPF must have exactly 11 digits.")
            .Must(CpfValidator.IsValid).WithMessage("Invalid CPF.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MinimumLength(8).WithMessage("Phone number must have at least 8 digits.")
            .MaximumLength(15).WithMessage("Phone number must not exceed 15 digits.");

        RuleFor(x => x.MonthlyIncome)
            .GreaterThan(0).WithMessage("Monthly income must be greater than zero.");
    }
}
