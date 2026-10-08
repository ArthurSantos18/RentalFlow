namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/reports")]
[ApiConventionType(typeof(ReportMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public sealed class ReportController(IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet("dashboard")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna o dashboard com métricas e informações resumidas.")]
    public async Task<IActionResult> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery();
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("top-property")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna as propriedades com maior número de aplicações de aluguel.")]
    public async Task<IActionResult> GetTopPropertyAsync([FromQuery] GetTopPropertiesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetTopPropertiesQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }
}