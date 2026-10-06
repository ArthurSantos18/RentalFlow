using RentalFlow.Application.UseCases.Queries.User;

namespace RentalFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
[ApiConventionType(typeof(UserMetadata))]
[EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
public class UserController(IQueryMediator _queryMediator) : ControllerBase
{
    [HttpGet("me")]
    [Stability(Stability.Stable)]
    [EndpointDescription("Obtém as informações do usuário atualmente autenticado.")]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(User);

        var query = new GetCurrentUserQuery(userId);
        var result = await _queryMediator.QueryAsync(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }
}
