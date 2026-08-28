using FluentValidation;
using RentalFlow.Application.Requests.Property;

namespace RentalFlow.Application.Validators.Property;

public sealed class GetPropertyByIdRequestValidator : AbstractValidator<GetPropertyByIdRequest>
{
    public GetPropertyByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Property ID is required.");
    }
}