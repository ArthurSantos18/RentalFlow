namespace RentalFlow.Infrastructure.Auditing;

public sealed class AuditSaveChangesInterceptor(IAuditLogFactory _auditLogFactory) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void AddAuditLogs(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries().Where(AuditEntityHelper.ShouldAudit).ToList();

        if (entries.Count == 0)
        {
            return;
        }

        var auditLogs = entries.SelectMany(_auditLogFactory.CreateLogs).ToList();

        context.Set<AuditLogEntity>().AddRange(auditLogs);
    }
}