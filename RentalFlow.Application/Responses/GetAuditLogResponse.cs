namespace RentalFlow.Application.Responses;

public sealed record GetAuditLogResponse
{
    [Description("O identificador único do registro de auditoria.")]
    public Guid Id { get; init; }

    [Description("A ação realizada na entidade auditada.")]
    public AuditAction Action { get; init; }

    [Description("O nome da entidade auditada.")]
    public string EntityName { get; init; } = string.Empty;
    
    [Description("O nome do campo auditado.")]
    public string? FieldName { get; init; }

    [Description("O valor antigo do campo auditado.")]
    public string? OldValue { get; init; }

    [Description("O valor novo do campo auditado.")]
    public string? NewValue { get; init; }

    [Description("O nome do usuário que realizou a ação.")]
    public string UserName { get; init; } = string.Empty;

    [Description("A data e hora em que o registro de auditoria foi criado.")]
    public DateTime CreatedAt { get; init; }
}
