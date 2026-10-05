namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/rental-applications")]
[Authorize]
[ApiConventionType(typeof(RentalApplicationMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public class RentalApplicationController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet]
    [EndpointDescription("Retorna uma lista de aplicações de aluguel com base nos critérios fornecidos.")]
    public async Task<IActionResult> GetRentalApplicationsAsync([FromQuery] GetRentalApplicationRequest request, CancellationToken cancellationToken)
    {
        var query = new GetRentalApplicationsQuery(request);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost]
    [EndpointDescription("Adiciona uma nova aplicação de aluguel ao sistema.")]
    public async Task<IActionResult> AddRentalApplicationAsync([FromBody] AddRentalApplicationRequest request, CancellationToken cancellationToken)
    {
        var command = new AddRentalApplicationCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Created(string.Empty, result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Deleta uma aplicação de aluguel existente do sistema.")]
    public async Task<IActionResult> DeleteRentalApplicationAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRentalApplicationCommand(id);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}")]
    [EndpointDescription("Atualiza uma aplicação de aluguel existente no sistema.")]
    public async Task<IActionResult> UpdateRentalApplicationAsync([FromRoute] Guid id, [FromBody] UpdateRentalApplicationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRentalApplicationCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = $"{nameof(OperatorRole.Administrator)},{nameof(OperatorRole.Manager)}")]
    [EndpointDescription("Atualiza o status de uma aplicação de aluguel existente no sistema.")]
    public async Task<IActionResult> UpdateRentalApplicationStatusAsync([FromRoute] Guid id, [FromBody] UpdateRentalApplicationStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}