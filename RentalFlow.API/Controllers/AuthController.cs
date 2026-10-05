namespace RentalFlow.API.Controllers;

[ApiController]
[Route("api/auth")]
[ApiConventionType(typeof(AuthMetadata))]
public class AuthController(ICommandMediator _commandMediator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.LoginPolicy)]
    [EndpointDescription("Autentica o usuário e retorna um token de acesso e um token de atualização.")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.RefreshPolicy)]
    [EndpointDescription("Atualiza o token de acesso usando o token de atualização.")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
    [EndpointDescription("Faz o logout do usuário e invalida o token de atualização.")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost("change-password")]
    [Authorize]
    [EnableRateLimiting(RateLimitingPolicies.AuthenticatedPolicy)]
    [EndpointDescription("Altera a senha do usuário autenticado.")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(User);

        var command = new ChangePasswordCommand(userId, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }
}