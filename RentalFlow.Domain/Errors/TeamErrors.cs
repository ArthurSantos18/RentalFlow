namespace RentalFlow.Domain.Errors;

public static class TeamErrors
{
    public static readonly Error TeamAlreadyExists = new(409, "Team already exists with this name.");
    public static readonly Error TeamNotFound = new(404, "The specified team was not found.");
    public static readonly Error TeamHasActiveOperators = new(409, "Cannot delete team because it has active operators.");
}