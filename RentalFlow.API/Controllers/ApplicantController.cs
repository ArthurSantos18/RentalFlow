using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using RentalFlow.API.Helpers;
using RentalFlow.Application.Requests;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Application.UseCases.Queries.Applicant;

namespace RentalFlow.API.Controllers;

[Route("api/applicant")]
[ApiController]
public class ApplicantController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllApplicantAsync(CancellationToken cancellationToken)
    {
        var query = new GetApplicantsQuery();
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpGet("cpf")]
    public async Task<IActionResult> GetApplicantByCpfAsync([FromQuery] GetApplicantByCpfRequest request, CancellationToken cancellationToken)
    {
        var query = new GetApplicantByCpfQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddApplicantAsync([FromBody] AddApplicantRequest request, CancellationToken cancellationToken)
    {
        var command = new AddApplicantCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteApplicantAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteApplicantCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateApplicantAsync([FromRoute] Guid id, [FromBody] UpdateApplicantRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateApplicantCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}

