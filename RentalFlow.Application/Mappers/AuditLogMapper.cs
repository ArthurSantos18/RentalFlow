namespace RentalFlow.Application.Mappers;

public static class AuditLogMapper
{
    public static GetAuditLogResponse ToResponse(this AuditLogEntity entity)
    {
        return new GetAuditLogResponse
        {
            Id = entity.Id,
            EntityName = entity.EntityName,
            FieldName = entity.FieldName,
            NewValue = entity.NewValue,
            OldValue = entity.OldValue,
            UserName = entity.UserName,
            Action = entity.Action,
            CreatedAt = entity.CreatedAt
        };
    }

    public static PagedResult<GetAuditLogResponse> ToResponse(this PagedResult<AuditLogEntity> pagedResult)
    {
        return new PagedResult<GetAuditLogResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(a => a.ToResponse())
        };
    }
}