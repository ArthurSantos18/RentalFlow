namespace RentalFlow.Application.Validators.AuditLog;

public sealed class GetAuditLogRequestValidator : AbstractValidator<GetAuditLogRequest>
{
    public GetAuditLogRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .IsInEnum()
            .WithMessage("O nome da entidade é obrigatório.");

        RuleFor(x => x.EntityId)
            .NotEmpty()
            .WithMessage("O ID da entidade é obrigatório.");
    }
}
