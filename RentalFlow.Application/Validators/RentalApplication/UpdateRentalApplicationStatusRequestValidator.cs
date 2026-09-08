using FluentValidation;
using RentalFlow.Application.Requests.RentalApplication;

namespace RentalFlow.Application.Validators.RentalApplication;

public sealed class UpdateRentalApplicationStatusRequestValidator : AbstractValidator<UpdateRentalApplicationStatusRequest>
{
    public UpdateRentalApplicationStatusRequestValidator()
    {
        RuleFor(x => x.RentalStatus)
            .IsInEnum()
            .WithMessage("Rental status must be a valid status.");
    }
}
