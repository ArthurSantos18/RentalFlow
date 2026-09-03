using FluentValidation;
using RentalFlow.Application.Requests.RentalApplication;

namespace RentalFlow.Application.Validators.RentalApplication;

public sealed class UpdateRentalApplicationRequestValidator : AbstractValidator<UpdateRentalApplicationRequest>
{
    public UpdateRentalApplicationRequestValidator()
    {
        RuleFor(x => x.Installments)
            .GreaterThan(0)
            .WithMessage("Installments must be greater than zero.")
            .When(x => x.Installments.HasValue);

        RuleFor(x => x.FinancedAmount)
            .GreaterThan(0)
            .WithMessage("Financed amount must be greater than zero.")
            .When(x => x.FinancedAmount.HasValue);

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero.")
            .When(x => x.TotalAmount.HasValue);

        RuleFor(x => x)
            .Must(x => !x.FinancedAmount.HasValue || !x.TotalAmount.HasValue || x.FinancedAmount <= x.TotalAmount)
            .WithMessage("Financed amount must be less than or equal to total amount.");

        RuleFor(x => x.ContractDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Contract date cannot be in the future.")
            .When(x => x.ContractDate.HasValue);
    }
}
