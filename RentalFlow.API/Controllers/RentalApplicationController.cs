using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RentalFlow.API.Helpers;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Application.UseCases.Queries.RentalApplication;

namespace RentalFlow.API.Controllers;

[Route("api/rental-application")]
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

    [HttpPatch("{id:guid}/applicant")]
    public async Task<IActionResult> ChangeRentalApplicationApplicantAsync([FromRoute] Guid id, [FromBody] ChangeRentalApplicationApplicantRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangeRentalApplicationApplicantCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}/operator")]
    public async Task<IActionResult> ChangeRentalApplicationOperatorAsync([FromRoute] Guid id, [FromBody] ChangeRentalApplicationOperatorRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangeRentalApplicationOperatorCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}/property")]
    public async Task<IActionResult> ChangeRentalApplicationPropertyAsync([FromRoute] Guid id, [FromBody] ChangeRentalApplicationPropertyRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangeRentalApplicationPropertyCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}
