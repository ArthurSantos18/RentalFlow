using FluentValidation;
using RentalFlow.Application.Requests;
using RentalFlow.Domain.Helpers;

namespace RentalFlow.Application.Validators;

public sealed class GetApplicantByCpfRequestValidator : AbstractValidator<GetApplicantByCpfRequest>
{
    public GetApplicantByCpfRequestValidator()
    {
        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required for search.")
            .Must(CpfValidator.IsValid).WithMessage("Invalid CPF.");
    }
}
