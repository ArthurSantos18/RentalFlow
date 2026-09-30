namespace RentalFlow.API.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid UserId => Guid.TryParse(User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty;

    public Guid OperatorId => Guid.TryParse(User?.FindFirst("operator_id")?.Value, out var id) ? id : Guid.Empty;

    public Guid? TeamId => Guid.TryParse(User?.FindFirst("team_id")?.Value, out var id) ? id : null;

    public string Email => User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}