using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RentalFlow.API.Helpers;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Application.UseCases.Queries.RentalApplication;

namespace RentalFlow.API.Controllers;

[Route("api/rental-applications")]
[ApiController]
public class RentalApplicationController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRentalApplicationsAsync([FromQuery] GetRentalApplicationRequest request, CancellationToken cancellationToken)
    {
        var query = new GetRentalApplicationsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddRentalApplicationAsync([FromBody] AddRentalApplicationRequest request, CancellationToken cancellationToken)
    {
        var command = new AddRentalApplicationCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRentalApplicationAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRentalApplicationCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateRentalApplicationAsync([FromRoute] Guid id, [FromBody] UpdateRentalApplicationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRentalApplicationCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateRentalApplicationStatusAsync([FromRoute] Guid id, [FromBody] UpdateRentalApplicationStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}
