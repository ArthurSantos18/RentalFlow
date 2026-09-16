namespace RentalFlow.Domain.Errors;

public static class UserErrors
{
    public static Error NewPasswordMustBeDifferent = new(400, "New password must be different from the current one.");
    public static Error InvalidCredentials = new(401, "Invalid email or password.");
    public static Error InvalidRefreshToken = new(401, "Invalid or expired refresh token.");
    public static Error UserInactive = new(403, "User is inactive.");
    public static Error MustChangePassword = new(403, "You must change your password before continuing.");
    public static Error UserNotFound = new(404, "User not found.");
    public static Error EmailAlreadyExists = new(409, "Email is already registered.");
}
