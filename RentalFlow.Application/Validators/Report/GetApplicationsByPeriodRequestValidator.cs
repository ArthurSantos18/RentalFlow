namespace RentalFlow.Application.Validators.Report;

public sealed class GetApplicationsByPeriodRequestValidator : AbstractValidator<GetApplicationsByPeriodRequest>
{
    public GetApplicationsByPeriodRequestValidator()
    {
        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("The start date is required.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("The end date is required.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("The end date must be greater than or equal to the start date.");

        RuleFor(x => x.GroupBy)
            .IsInEnum()
            .WithMessage("The group by is invalid.");
    }
}
