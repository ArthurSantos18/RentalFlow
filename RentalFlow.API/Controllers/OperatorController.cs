using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RentalFlow.API.Helpers;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.UseCases.Commands.Operator;
using RentalFlow.Application.UseCases.Queries.Operator;

namespace RentalFlow.API.Controllers;

[Route("api/operator")]
[ApiController]

public sealed class OperatorController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOperatorsAsync([FromQuery] GetOperatorsRequest request, CancellationToken cancellationToken)
    {
        var query = new GetOperatorsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddOperatorAsync([FromBody] AddOperatorRequest request, CancellationToken cancellationToken)
    {
        var command = new AddOperatorCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOperatorAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteOperatorCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateOperatorAsync([FromRoute] Guid id, [FromBody] UpdateOperatorRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOperatorCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}
