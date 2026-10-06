namespace RentalFlow.Application.Interfaces.Repositories;

public interface IAuditLogRepository
{
    Task<PagedResult<AuditLogEntity>> GetAuditLogsAsync(GetAuditLogRequest request, CancellationToken cancellationToken);
}
