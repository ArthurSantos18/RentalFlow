using FluentValidation;
using RentalFlow.Application.Requests.RentalApplication;

namespace RentalFlow.Application.Validators.RentalApplication;

public sealed class GetRentalApplicationRequestValidator : AbstractValidator<GetRentalApplicationRequest>
{
    public GetRentalApplicationRequestValidator()
    {
        RuleFor(x => x.MinFinancedAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinFinancedAmount.HasValue)
            .WithMessage("Minimum financed amount cannot be negative.");

        RuleFor(x => x.MaxFinancedAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxFinancedAmount.HasValue)
            .WithMessage("Maximum financed amount cannot be negative.");

        RuleFor(x => x)
            .Must(x => !x.MinFinancedAmount.HasValue || !x.MaxFinancedAmount.HasValue || x.MinFinancedAmount <= x.MaxFinancedAmount)
            .WithMessage("Minimum financed amount cannot be greater than maximum financed amount.");

        RuleFor(x => x.MinTotalAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinTotalAmount.HasValue)
            .WithMessage("Minimum total amount cannot be negative.");

        RuleFor(x => x.MaxTotalAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxTotalAmount.HasValue)
            .WithMessage("Maximum total amount cannot be negative.");

        RuleFor(x => x)
            .Must(x => !x.MinTotalAmount.HasValue || !x.MaxTotalAmount.HasValue || x.MinTotalAmount <= x.MaxTotalAmount)
            .WithMessage("Minimum total amount cannot be greater than maximum total amount.");

        RuleFor(x => x)
            .Must(x => !x.MinCreatedAt.HasValue || !x.MaxCreatedAt.HasValue || x.MinCreatedAt <= x.MaxCreatedAt)
            .WithMessage("Minimum created date cannot be greater than maximum created date.");

        RuleFor(x => x)
            .Must(x => !x.MinContractDate.HasValue || !x.MaxContractDate.HasValue || x.MinContractDate <= x.MaxContractDate)
            .WithMessage("Minimum contract date cannot be greater than maximum contract date.");

        RuleForEach(x => x.ProposalNumbers)
            .NotEmpty()
            .WithMessage("Proposal number cannot be empty.")
            .MaximumLength(50)
            .WithMessage("Proposal number must not exceed 50 characters.")
            .When(x => x.ProposalNumbers?.Any() == true);
    }
}
