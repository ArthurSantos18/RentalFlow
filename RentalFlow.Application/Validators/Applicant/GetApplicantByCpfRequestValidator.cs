using FluentValidation;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Helpers;

namespace RentalFlow.Application.Validators.Applicant;

public sealed class GetApplicantByCpfRequestValidator : AbstractValidator<GetApplicantByCpfRequest>
{
    public GetApplicantByCpfRequestValidator()
    {
        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required for search.")
            .Must(CpfValidator.IsValid).WithMessage("Invalid CPF.");
    }
}
