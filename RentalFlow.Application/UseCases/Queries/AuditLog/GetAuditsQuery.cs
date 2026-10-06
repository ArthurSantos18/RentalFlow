namespace RentalFlow.Application.UseCases.Queries.AuditLog;

public sealed record GetAuditsQuery(GetAuditLogRequest Request) : IQuery<Result<PagedResult<GetAuditLogResponse>>>;
