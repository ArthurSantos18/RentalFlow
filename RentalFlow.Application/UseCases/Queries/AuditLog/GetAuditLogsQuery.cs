namespace RentalFlow.Application.UseCases.Queries.AuditLog;

public sealed record GetAuditLogsQuery(GetAuditLogRequest Request) : IQuery<Result<PagedResult<GetAuditLogResponse>>>;
