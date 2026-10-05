namespace RentalFlow.Domain.Errors;

public static class UserErrors
{
    public static readonly Error InvalidRole = new(403, "The current user's role is not authorized to perform this action.");
    public static readonly Error InvalidCredentials = new(401, "Invalid email or password.");
    public static readonly Error InvalidPassword = new(401, "Invalid password.");
    public static readonly Error InvalidRefreshToken = new(401, "Invalid or expired refresh token.");
    public static readonly Error MustChangePassword = new(403, "You must change your password before continuing.");
    public static readonly Error UserNotFound = new(404, "User not found.");
    public static readonly Error EmailAlreadyExists = new(409, "Email is already registered.");
}