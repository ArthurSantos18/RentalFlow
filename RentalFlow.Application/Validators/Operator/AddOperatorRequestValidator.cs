using FluentValidation;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Validators.Operator;

public sealed class AddOperatorRequestValidator : AbstractValidator<AddOperatorRequest>
{
    public AddOperatorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Role)
            .NotEqual(OperatorRole.None)
            .WithMessage("Role is required.");

        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("TeamId is required.");
    }
}
