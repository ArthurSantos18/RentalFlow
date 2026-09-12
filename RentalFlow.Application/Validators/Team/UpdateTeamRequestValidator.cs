using FluentValidation;
using RentalFlow.Application.Requests.Team;

namespace RentalFlow.Application.Validators.Team;

public sealed class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
{
    public UpdateTeamRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .WithMessage("Name must have at least 3 characters.")
            .MaximumLength(30)
            .WithMessage("Name must not exceed 30 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MinimumLength(3)
            .WithMessage("Description must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Description must not exceed 100 characters.")
            .When(x => x.Description is not null);
    }
}