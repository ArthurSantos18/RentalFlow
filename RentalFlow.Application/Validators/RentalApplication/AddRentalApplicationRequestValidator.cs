using FluentValidation;
using RentalFlow.Application.Requests.RentalApplication;

namespace RentalFlow.Application.Validators.RentalApplication;

public sealed class AddRentalApplicationRequestValidator : AbstractValidator<AddRentalApplicationRequest>
{
    public AddRentalApplicationRequestValidator()
    {
        RuleFor(x => x.ApplicantId)
            .NotEmpty()
            .WithMessage("ApplicantId is required.");

        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .WithMessage("PropertyId is required.");

        RuleFor(x => x.OperatorId)
            .NotEmpty()
            .WithMessage("OperatorId is required.");

        RuleFor(x => x.FinancedAmount)
            .GreaterThan(0)
            .WithMessage("Financed amount must be greater than zero.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero.");

        RuleFor(x => x.Installments)
            .GreaterThan(0)
            .WithMessage("Installments must be greater than zero.");

        RuleFor(x => x)
            .Must(x => x.FinancedAmount <= x.TotalAmount)
            .WithMessage("Financed amount must be less than or equal to total amount.");

        RuleFor(x => x.ContractDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Contract date cannot be in the future.")
            .When(x => x.ContractDate.HasValue);
    }
}
