namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/applicants")]
[Authorize]
[ApiConventionType(typeof(ApplicantMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public sealed class ApplicantController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    [Stability(Stability.Stable)]
    [EndpointDescription("Retorna uma lista de inquilinos com base nos critérios fornecidos.")]
    public async Task<IActionResult> GetApplicantsAsync([FromQuery] GetApplicantRequest request, CancellationToken cancellationToken)
    {
        var query = new GetApplicantsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    [Stability(Stability.Stable)]
    [EndpointDescription("Adiciona um novo inquilino ao sistema.")]
    public async Task<IActionResult> AddApplicantAsync([FromBody] AddApplicantRequest request, CancellationToken cancellationToken)
    {
        var command = new AddApplicantCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [Stability(Stability.Stable)]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Deleta um inquilino existente no sistema.")]
    public async Task<IActionResult> DeleteApplicantAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteApplicantCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    [Stability(Stability.Stable)]
    [EndpointDescription("Atualiza um inquilino existente no sistema.")]
    public async Task<IActionResult> UpdateApplicantAsync([FromRoute] Guid id, [FromBody] UpdateApplicantRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateApplicantCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}