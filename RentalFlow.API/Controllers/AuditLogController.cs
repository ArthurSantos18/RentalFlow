namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
[ApiConventionType(typeof(AuditLogMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public sealed class AuditLogController(IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna uma lista de logs de auditoria com base nos critérios fornecidos.")]
    public async Task<IActionResult> GetAuditLogAsync([FromQuery] GetAuditLogRequest request, CancellationToken cancellationToken)
    {
        var query = new GetAuditsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }
}
