#pragma warning disable IDE0060

namespace RentalFlow.API.Metadata;

public static class AuditLogMetadata
{
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(PagedResult<GetAuditLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    public static void GetAuditLogAsync(GetAuditLogRequest request, CancellationToken cancellationToken) { }
}
