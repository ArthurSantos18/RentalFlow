namespace RentalFlow.Application.Validators.Report;

public sealed class GetConversionRateRequestValidator : AbstractValidator<GetConversionRateRequest>
{
    public GetConversionRateRequestValidator()
    {
        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("The start date is required.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("The end date is required.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("The end date must be greater than or equal to the start date.");
    }
}
