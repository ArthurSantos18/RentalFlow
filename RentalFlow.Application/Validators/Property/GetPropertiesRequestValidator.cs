using FluentValidation;
using RentalFlow.Application.Requests.Property;

namespace RentalFlow.Application.Validators.Property;

public sealed class GetPropertiesRequestValidator : AbstractValidator<GetPropertyRequest>
{
    public GetPropertiesRequestValidator()
    {
        RuleFor(x => x.MinRentPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinRentPrice.HasValue)
            .WithMessage("Minimum rent price cannot be negative.");

        RuleFor(x => x.MaxRentPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxRentPrice.HasValue)
            .WithMessage("Maximum rent price cannot be negative.");

        RuleFor(x => x)
            .Must(x => !x.MinRentPrice.HasValue || !x.MaxRentPrice.HasValue || x.MinRentPrice <= x.MaxRentPrice)
            .WithMessage("Minimum rent price cannot be greater than maximum rent price.");

        RuleFor(x => x.MinBedrooms)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinBedrooms.HasValue)
            .WithMessage("Minimum number of bedrooms cannot be negative.");

        RuleFor(x => x.MaxBedrooms)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxBedrooms.HasValue)
            .WithMessage("Maximum number of bedrooms cannot be negative.");

        RuleFor(x => x)
            .Must(x => !x.MinBedrooms.HasValue || !x.MaxBedrooms.HasValue || x.MinBedrooms <= x.MaxBedrooms)
            .WithMessage("Minimum number of bedrooms cannot be greater than maximum number of bedrooms.");

        RuleForEach(x => x.ZipCodes)
            .Matches(@"^\d{5}-?\d{3}$")
            .When(x => x.ZipCodes?.Any() == true)
            .WithMessage("Invalid ZIP code format. Use XXXXX-XXX or XXXXXXXX.");
    }
}
