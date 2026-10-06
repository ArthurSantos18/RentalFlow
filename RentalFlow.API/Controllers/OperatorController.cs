namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/operators")]
[Authorize]
[ApiConventionType(typeof(OperatorMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public sealed class OperatorController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    [Stability(Stability.Stable)]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Retorna uma lista de operadores com base nos critérios fornecidos.")]
    public async Task<IActionResult> GetOperatorsAsync([FromQuery] GetOperatorRequest request, CancellationToken cancellationToken)
    {
        var query = new GetOperatorsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("{id:guid}")]
    [Stability(Stability.Stable)]
    [Authorize]
    [EndpointDescription("Retorna um operador pelo seu identificador único.")]
    public async Task<IActionResult> GetOperatorByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOperatorByIdQuery(id);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    [Stability(Stability.Stable)]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Adiciona um novo operador ao sistema.")]
    public async Task<IActionResult> AddOperatorAsync([FromBody] AddOperatorRequest request, CancellationToken cancellationToken)
    {
        var command = new AddOperatorCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [Stability(Stability.Stable)]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [EndpointDescription("Deleta um operador existente do sistema.")]
    public async Task<IActionResult> DeleteOperatorAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteOperatorCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    [Stability(Stability.Stable)]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Atualiza um operador existente no sistema.")]
    public async Task<IActionResult> UpdateOperatorAsync([FromRoute] Guid id, [FromBody] UpdateOperatorRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOperatorCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{operatorId:guid}/team/{teamId:guid}")]
    [Stability(Stability.Stable)]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Atribui um operador a uma equipe específica.")]
    public async Task<IActionResult> AssignOperatorAsync([FromRoute] Guid operatorId, [FromRoute] Guid teamId, CancellationToken cancellationToken)
    {
        var command = new AssignOperatorCommand(operatorId, teamId);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}