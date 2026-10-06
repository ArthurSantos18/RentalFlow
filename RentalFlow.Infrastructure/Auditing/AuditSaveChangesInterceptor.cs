namespace RentalFlow.Infrastructure.Auditing;

public sealed class AuditSaveChangesInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        AddAuditLogs(context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;

        if (context is not null)
        {
            AddAuditLogs(context);
        }

        return base.SavingChanges(eventData, result);
    }

    private void AddAuditLogs(DbContext context)
    {
        var entries = context.ChangeTracker.Entries().Where(ShouldAudit).ToList();

        if (entries.Count == 0)
        {
            return;
        }

        var auditLogs = new List<AuditLogEntity>();

        foreach (var entry in entries)
        {
            auditLogs.AddRange(CreateAuditLogs(entry));
        }

        context.Set<AuditLogEntity>().AddRange(auditLogs);
    }

    private List<AuditLogEntity> CreateAuditLogs(EntityEntry entry)
    {
        var auditLogs = new List<AuditLogEntity>();

        var entityName = entry.Metadata.ClrType.Name;

        var entityId = GetEntityId(entry);

        if (entityId == Guid.Empty)
        {
            return auditLogs;
        }

        var action = GetAuditAction(entry.State);

        if (entry.State == EntityState.Modified)
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

                if (ShouldIgnoreProperty(property.Metadata))
                {
                    continue;
                }

                var oldValue = SerializeValue(property.OriginalValue, property.Metadata.Name);
                var newValue = SerializeValue(property.CurrentValue, property.Metadata.Name);

                if (oldValue == newValue)
                {
                    continue;
                }

                auditLogs.Add(new AuditLogEntity(
                    entityName,
                    entityId,
                    property.Metadata.Name,
                    oldValue,
                    newValue,
                    action,
                    currentUserService.UserId,
                    currentUserService.Name));
            }

            return auditLogs;
        }

        auditLogs.Add(new AuditLogEntity(
            entityName,
            entityId,
            null,
            null,
            null,
            action,
            currentUserService.UserId,
            currentUserService.Name));

        return auditLogs;
    }

    private static bool ShouldAudit(EntityEntry entry)
    {
        return entry.Entity is not AuditLogEntity && entry.State is
            EntityState.Added or
            EntityState.Modified or
            EntityState.Deleted;
    }

    private static AuditAction GetAuditAction(EntityState state)
    {
        return state switch
        {
            EntityState.Added => AuditAction.Created,
            EntityState.Modified => AuditAction.Modified,
            EntityState.Deleted => AuditAction.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };
    }

    private static Guid GetEntityId(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();

        if (primaryKey is null || primaryKey.Properties.Count != 1)
        {
            return Guid.Empty;
        }

        var property = primaryKey.Properties[0];

        var value = entry.Property(property.Name).CurrentValue;

        return value switch
        {
            Guid guid => guid,

            string stringValue when Guid.TryParse(stringValue, out var guid) => guid,

            _ => Guid.Empty
        };
    }

    private static string? SerializeValue(object? value, string fieldName)
    {
        if (value is null)
        {
            return null;
        }

        if (value is string stringValue)
        {
            return MaskHelper.Mask(fieldName, stringValue);
        }

        if (value is DateTime dateTime)
        {
            return dateTime.ToString("O", CultureInfo.InvariantCulture);
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("O", CultureInfo.InvariantCulture);
        }

        if (value is Guid guid)
        {
            return guid.ToString();
        }

        if (value is Enum enumValue)
        {
            return enumValue.ToString();
        }

        return JsonSerializer.Serialize(value, JsonOptions);
    }

    private static bool ShouldIgnoreProperty(IProperty property)
    {
        return property.PropertyInfo?.IsDefined(typeof(IgnoreAuditAttribute), inherit: true) ?? false;
    }
}