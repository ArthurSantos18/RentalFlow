namespace RentalFlow.Application.UseCases.Queries.AuditLog;

public sealed class GetAuditLogsQueryHandler(IAuditLogRepository _auditRepository) : IQueryHandler<GetAuditLogsQuery, Result<PagedResult<GetAuditLogResponse>>>
{
    public async Task<Result<PagedResult<GetAuditLogResponse>>> HandleAsync(GetAuditLogsQuery query, CancellationToken cancellationToken)
    {
        var result = await _auditRepository.GetAuditLogsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetAuditLogResponse>>.Success(result.ToResponse());
    }
}
