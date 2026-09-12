using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class TeamErrors
{
    public static Error TeamDoesExist = new(409, "Team already exists with this name.");
    public static Error TeamNotFound => new(404, "The specified team was not found.");
}
