namespace RentalFlow.Domain.Errors;

public static class TeamErrors
{
    public static readonly Error TeamDoesExist = new(409, "Team already exists with this name.");
    public static readonly Error TeamInactive = new(409, "Team is Inactive");
    public static readonly Error TeamNotFound = new(404, "The specified team was not found.");
}
