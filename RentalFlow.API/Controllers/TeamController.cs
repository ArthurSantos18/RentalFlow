namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
[ApiConventionType(typeof(TeamMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public sealed class TeamController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("Retorna uma lista de equipes com base nos critérios fornecidos.")]
    public async Task<IActionResult> GetTeamsAsync([FromQuery] GetTeamRequest request, CancellationToken cancellationToken)
    {
        var query = new GetTeamsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [EndpointDescription("Adiciona uma nova equipe ao sistema.")]
    public async Task<IActionResult> AddTeamAsync([FromBody] AddTeamRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTeamCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
    [EndpointDescription("Deleta uma equipe existente do sistema.")]
    public async Task<IActionResult> DeleteTeamAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTeamCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Atualiza uma equipe existente no sistema.")]
    public async Task<IActionResult> UpdateTeamAsync([FromRoute] Guid id, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTeamCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}