using FluentValidation;
using RentalFlow.Application.Requests.Property;

namespace RentalFlow.Application.Validators.Property;

public sealed class UpdatePropertyRequestValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyRequestValidator()
    {
        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .MaximumLength(100).WithMessage("Street must not exceed 100 characters.")
                .When(x => x.Address!.Street is not null);

            RuleFor(x => x.Address!.Number)
                .MaximumLength(10).WithMessage("Number must not exceed 10 characters.")
                .When(x => x.Address!.Number is not null);

            RuleFor(x => x.Address!.Complement)
                .MaximumLength(50).WithMessage("Complement must not exceed 50 characters.")
                .When(x => x.Address!.Complement is not null);

            RuleFor(x => x.Address!.Neighborhood)
                .MaximumLength(50).WithMessage("Neighborhood must not exceed 50 characters.")
                .When(x => x.Address!.Neighborhood is not null);

            RuleFor(x => x.Address!.City)
                .MaximumLength(50).WithMessage("City must not exceed 50 characters.")
                .When(x => x.Address!.City is not null);

            RuleFor(x => x.Address!.State)
                .Length(2).WithMessage("State must be a 2-letter code (e.g., BA).")
                .Must(state => state?.All(char.IsLetter) ?? true)
                .WithMessage("State must contain only letters.")
                .When(x => x.Address!.State is not null);

            RuleFor(x => x.Address!.ZipCode)
                .Length(8).WithMessage("ZipCode must have exactly 8 digits.")
                .Matches("^[0-9]+$").WithMessage("ZipCode must contain only numbers.")
                .When(x => x.Address!.ZipCode is not null);
        });

        RuleFor(x => x.RentPrice)
            .GreaterThan(0).WithMessage("Rent price must be greater than zero.")
            .When(x => x.RentPrice.HasValue);

        RuleFor(x => x.Bedrooms)
            .GreaterThan(0).WithMessage("Bedrooms must be greater than zero.")
            .When(x => x.Bedrooms.HasValue);

        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Availability status is required.")
            .When(x => x.IsAvailable.HasValue);

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("Active status is required.")
            .When(x => x.IsActive.HasValue);
    }
}
