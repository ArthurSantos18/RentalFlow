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

    [HttpGet("top-properties")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna as propriedades com maior número de aplicações de aluguel.")]
    public async Task<IActionResult> GetTopPropertiesAsync([FromQuery] GetTopPropertiesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetTopPropertiesQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("top-operators")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna os operadores com maior número de aplicações de aluguel.")]
    public async Task<IActionResult> GetTopOperatorsAsync([FromQuery] GetTopOperatorsRequest request, CancellationToken cancellationToken)
    {
        var query = new GetTopOperatorsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("applications-by-period")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna o número de aplicações de aluguel por período.")]
    public async Task<IActionResult> GetApplicationsByPeriodAsync([FromQuery] GetApplicationsByPeriodRequest request, CancellationToken cancellationToken)
    {
        var query = new GetApplicationsByPeriodQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("conversion-rate")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna a taxa de conversão das aplicações de aluguel no período informado.")]
    public async Task<IActionResult> GetConversionRateAsync([FromQuery] GetConversionRateRequest request, CancellationToken cancellationToken)
    {
        var query = new GetConversionRateQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }
}