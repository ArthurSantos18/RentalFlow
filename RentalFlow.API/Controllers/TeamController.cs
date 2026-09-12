using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RentalFlow.API.Helpers;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.UseCases.Commands.Team;
using RentalFlow.Application.UseCases.Queries.Team;

namespace RentalFlow.API.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> AddTeamAsync([FromBody] AddTeamRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTeamCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeamAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTeamCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateTeamAsync([FromRoute] Guid id, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTeamCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}
