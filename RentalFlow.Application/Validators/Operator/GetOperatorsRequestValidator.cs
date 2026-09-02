using FluentValidation;
using RentalFlow.Application.Requests.Operator;

namespace RentalFlow.Application.Validators.Operator;

public sealed class GetOperatorsRequestValidator : AbstractValidator<GetOperatorsRequest>
{
    public GetOperatorsRequestValidator()
    {
        RuleForEach(x => x.Ids)
            .NotEmpty()
            .WithMessage("Operator ID cannot be empty.")
            .When(x => x.Ids?.Any() == true);

        RuleForEach(x => x.Names)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MinimumLength(3)
            .WithMessage("Name must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .When(x => x.Names?.Any() == true);

        RuleForEach(x => x.Emails)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.")
            .When(x => x.Emails?.Any() == true);

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid operator role.")
            .When(x => x.Role.HasValue);

        RuleForEach(x => x.ApplicationIds)
            .NotEmpty()
            .WithMessage("Application ID cannot be empty.")
            .When(x => x.ApplicationIds?.Any() == true);
    }
}
