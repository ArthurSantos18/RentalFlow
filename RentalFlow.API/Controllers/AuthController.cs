namespace RentalFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ICommandMediator _commandMediator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : ApiResponseHelper.HandleError(result.Error);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(User);

        var command = new ChangePasswordCommand(userId, request);
        var result = await _commandMediator.SendAsync(command, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ApiResponseHelper.HandleError(result.Error);
    }
}
