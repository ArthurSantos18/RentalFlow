using FluentValidation;
using RentalFlow.Application.Requests.Team;

namespace RentalFlow.Application.Validators.Team;

public sealed class GetTeamsRequestValidator : AbstractValidator<GetTeamRequest>
{
    public GetTeamsRequestValidator()
    {
        RuleForEach(x => x.Ids)
            .NotEmpty()
            .WithMessage("Team ID cannot be empty.")
            .When(x => x.Ids?.Any() == true);

        RuleForEach(x => x.Names)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MinimumLength(3)
            .WithMessage("Name must have at least 3 characters.")
            .MaximumLength(30)
            .WithMessage("Name must not exceed 30 characters.")
            .When(x => x.Names?.Any() == true);

        RuleForEach(x => x.Description)
            .NotEmpty()
            .WithMessage("Description cannot be empty.")
            .MinimumLength(3)
            .WithMessage("Description must have at least 3 characters.")
            .MaximumLength(100)
            .WithMessage("Description must not exceed 100 characters.")
            .When(x => x.Description?.Any() == true);
    }
}
