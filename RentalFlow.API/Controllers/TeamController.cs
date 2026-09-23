namespace RentalFlow.API.Controllers;

[Authorize]
[Route("api/teams")]
[ApiController]
public sealed class TeamController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTeamsAsync([FromQuery] GetTeamRequest request, CancellationToken cancellationToken)
    {
        var query = new GetTeamsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("{id:guid}/operators")]
    public async Task<IActionResult> GetTeamOperators([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var request = new GetOperatorRequest { TeamIds = [id] };
        var query = new GetOperatorsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = nameof(OperatorRole.Administrator))]
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
    public async Task<IActionResult> UpdateTeamAsync([FromRoute] Guid id, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTeamCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}
