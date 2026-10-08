namespace RentalFlow.Application.Validators.Report;

public sealed class GetTopPropertiesRequestValidator : AbstractValidator<GetTopPropertiesRequest>
{
    public GetTopPropertiesRequestValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50)
            .WithMessage("O limite deve estar entre 1 e 50.");

        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From!.Value)
            .WithMessage("A data final deve ser maior ou igual à data inicial.")
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}