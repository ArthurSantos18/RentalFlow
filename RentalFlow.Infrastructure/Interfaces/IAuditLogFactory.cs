namespace RentalFlow.Infrastructure.Interfaces;

public interface IAuditLogFactory
{
    IReadOnlyList<AuditLogEntity> CreateLogs(EntityEntry entry);
}
