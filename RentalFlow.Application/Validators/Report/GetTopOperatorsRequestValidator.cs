namespace RentalFlow.Application.Validators.Report;

public sealed class GetTopOperatorsRequestValidator : AbstractValidator<GetTopOperatorsRequest>
{
    public GetTopOperatorsRequestValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50)
            .WithMessage("The limit must be between 1 and 50.");

        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From!.Value)
            .WithMessage("The end date must be greater than or equal to the start date.")
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}