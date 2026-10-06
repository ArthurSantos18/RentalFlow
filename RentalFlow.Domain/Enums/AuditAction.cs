namespace RentalFlow.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuditAction
{
    None = 0,
    Created = 1,
    Modified = 2,
    Deleted = 3
}
