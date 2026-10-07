namespace RentalFlow.Infrastructure.Factories;

public sealed class AuditLogFactory(ICurrentUserService _currentUserService) : IAuditLogFactory
{
    public IReadOnlyList<AuditLogEntity> CreateLogs(EntityEntry entry)
    {
        var auditLogs = new List<AuditLogEntity>();

        var entityName = entry.Metadata.ClrType.Name;
        var entityId = AuditEntityHelper.GetEntityId(entry);

        if (entityId == Guid.Empty)
        {
            return auditLogs;
        }

        var action = AuditEntityHelper.GetAuditAction(entry.State);

        if (entry.State == EntityState.Modified)
        {
            AddModifiedFieldAuditLogs(auditLogs, entry, entityName, entityId, action);
            return auditLogs;
        }

        auditLogs.Add(BuildAuditLog(entityName, entityId, action));

        return auditLogs;
    }

    private void AddModifiedFieldAuditLogs(List<AuditLogEntity> auditLogs, EntityEntry entry, string entityName, Guid entityId, AuditAction action)
    {
        foreach (var property in entry.Properties)
        {
            if (!property.IsModified)
            {
                continue;
            }

            if (property.Metadata.IsPrimaryKey())
            {
                continue;
            }

            if (AuditEntityHelper.ShouldIgnoreProperty(property.Metadata))
            {
                continue;
            }

            var oldValue = AuditEntityHelper.Serialize(property.OriginalValue, property.Metadata.Name);
            var newValue = AuditEntityHelper.Serialize(property.CurrentValue, property.Metadata.Name);

            if (oldValue == newValue)
            {
                continue;
            }

            auditLogs.Add(BuildAuditLog(entityName, entityId, action, property.Metadata.Name, oldValue, newValue));
        }
    }

    private AuditLogEntity BuildAuditLog(string entityName, Guid entityId, AuditAction action, string? fieldName = null, string? oldValue = null, string? newValue = null)
    {
        return new AuditLogEntity(
            entityName,
            entityId,
            fieldName,
            oldValue,
            newValue,
            action,
            _currentUserService.UserId,
            _currentUserService.Name);
    }
}