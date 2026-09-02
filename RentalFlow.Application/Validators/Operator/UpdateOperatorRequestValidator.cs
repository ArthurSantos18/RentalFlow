using FluentValidation;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Validators.Operator;

public sealed class UpdateOperatorRequestValidator : AbstractValidator<UpdateOperatorRequest>
{
    public UpdateOperatorRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Name must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(100)
            .WithMessage("Email must not exceed 100 characters.")
            .When(x => x.Email is not null);

        RuleFor(x => x.Role)
            .NotEqual(OperatorRole.None)
            .WithMessage("Role is required.")
            .When(x => x.Role.HasValue);
    }
}