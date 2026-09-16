namespace RentalFlow.API.Helpers;

public static class ClaimsPrincipalHelper
{
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");

        if (!Guid.TryParse(userId, out var id))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return id;
    }
}
