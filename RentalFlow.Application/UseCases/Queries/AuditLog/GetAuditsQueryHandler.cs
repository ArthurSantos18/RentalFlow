namespace RentalFlow.Application.UseCases.Queries.AuditLog;

public sealed class GetAuditsQueryHandler(IAuditLogRepository _auditRepository) : IQueryHandler<GetAuditsQuery, Result<PagedResult<GetAuditLogResponse>>>
{
    public async Task<Result<PagedResult<GetAuditLogResponse>>> HandleAsync(GetAuditsQuery query, CancellationToken cancellationToken)
    {
        var result = await _auditRepository.GetAuditLogsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetAuditLogResponse>>.Success(result.ToResponse());
    }
}
