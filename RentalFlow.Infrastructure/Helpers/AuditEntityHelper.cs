namespace RentalFlow.Infrastructure.Helpers;

public static class AuditEntityHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static bool ShouldAudit(EntityEntry entry)
    {
        if (entry.Entity is AuditLogEntity)
        {
            return false;
        }

        return entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted;
    }

    public static bool ShouldIgnoreProperty(IProperty property)
    {
        return property.PropertyInfo?.IsDefined(typeof(IgnoreAuditAttribute), inherit: true) ?? false;
    }

    public static AuditAction GetAuditAction(EntityState state)
    {
        return state switch
        {
            EntityState.Added => AuditAction.Created,
            EntityState.Modified => AuditAction.Modified,
            EntityState.Deleted => AuditAction.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };
    }

    public static Guid GetEntityId(EntityEntry entry)
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

    public static string? Serialize(object? value, string fieldName)
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
}