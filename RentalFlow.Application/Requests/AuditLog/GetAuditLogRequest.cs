namespace RentalFlow.Application.Requests.AuditLog;

public sealed record GetAuditLogRequest
{
    public GetAuditLogRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("O nome da entidade para filtrar.")]
    public AuditEntity EntityName { get; init; }

    [Description("O ID da entidade para filtrar.")]
    public Guid EntityId { get; init; }
}
