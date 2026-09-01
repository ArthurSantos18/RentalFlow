using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RentalFlow.API.Helpers;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.UseCases.Commands.Property;
using RentalFlow.Application.UseCases.Queries.Property;

namespace RentalFlow.API.Controllers;

[Route("api/property")]
[ApiController]
public sealed class PropertyController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProperties([FromQuery] GetPropertiesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetPropertiesQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddPropertyAsync([FromBody] AddPropertyRequest request, CancellationToken cancellationToken)
    {
        var command = new AddPropertyCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePropertyAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePropertyCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateApplicantAsync([FromRoute] Guid id, [FromBody] UpdatePropertyRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePropertyCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}
