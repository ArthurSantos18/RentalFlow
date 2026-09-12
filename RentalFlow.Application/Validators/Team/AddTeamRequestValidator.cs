using FluentValidation;
using RentalFlow.Application.Requests.Team;

namespace RentalFlow.Application.Validators.Team;

internal class AddTeamRequestValidator : AbstractValidator<AddTeamRequest>
{
    public AddTeamRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must have at least 3 characters.")
            .MaximumLength(30)
            .WithMessage("Name must not exceed 30 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MinimumLength(3)
            .WithMessage("Description must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Description must not exceed 100 characters.");
    }
}
