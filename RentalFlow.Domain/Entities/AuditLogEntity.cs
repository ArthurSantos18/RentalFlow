namespace RentalFlow.Domain.Entities;

public sealed class AuditLogEntity
{
    public Guid Id { get; private set; }

    public string EntityName { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    public string? FieldName { get; private set; }

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    public AuditAction Action { get; private set; }

    public Guid UserId { get; private set; }

    public string UserName { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private AuditLogEntity()
    {
    }

    public AuditLogEntity(
        string entityName,
        Guid entityId,
        string? fieldName,
        string? oldValue,
        string? newValue,
        AuditAction action,
        Guid userId,
        string userName)
    {
        Id = Guid.NewGuid();
        EntityName = entityName;
        EntityId = entityId;
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
        Action = action;
        UserId = userId;
        UserName = userName;
        CreatedAt = DateTime.UtcNow;
    }
}