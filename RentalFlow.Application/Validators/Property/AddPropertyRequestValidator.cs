using FluentValidation;
using RentalFlow.Application.Requests.Property;

namespace RentalFlow.Application.Validators.Property;

public sealed class AddPropertyRequestValidator : AbstractValidator<AddPropertyRequest>
{
    public AddPropertyRequestValidator()
    {
        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required.");

        RuleFor(x => x.Address.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(100).WithMessage("Street must not exceed 100 characters.");

        RuleFor(x => x.Address.Number)
            .NotEmpty().WithMessage("Number is required.")
            .MaximumLength(10).WithMessage("Number must not exceed 10 characters.");

        RuleFor(x => x.Address.Neighborhood)
            .NotEmpty().WithMessage("Neighborhood is required.")
            .MaximumLength(50).WithMessage("Neighborhood must not exceed 50 characters.");

        RuleFor(x => x.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(50).WithMessage("City must not exceed 50 characters.");

        RuleFor(x => x.Address.State)
            .NotEmpty().WithMessage("State is required.")
            .Length(2).WithMessage("State must be a 2-letter code (e.g., BA).")
            .Must(state => state.All(char.IsLetter)).WithMessage("State must contain only letters.");

        RuleFor(x => x.Address.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required.")
            .Length(8).WithMessage("ZipCode must have exactly 8 digits.")
            .Matches("^[0-9]+$").WithMessage("ZipCode must contain only numbers.");

        RuleFor(x => x.RentPrice)
                   .GreaterThan(0).WithMessage("Rent price must be greater than zero.");

        RuleFor(x => x.RentPrice)
            .GreaterThan(0).WithMessage("Rent price must be greater than zero.");

        RuleFor(x => x.Bedrooms)
            .GreaterThan(0).WithMessage("Bedrooms must be greater than zero.");

        RuleFor(x => x.IsAvaiable)
            .NotNull().WithMessage("IsAvailable is required.");
    }
}
