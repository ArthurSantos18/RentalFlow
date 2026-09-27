namespace RentalFlow.Domain.Errors;

public static class OperatorErrors
{
    public static readonly Error OperatorNotFound = new(404, "Operator not found.");
    public static readonly Error OperatorIsInactive = new(409, "Operator is inactive.");
    public static readonly Error OperatorNotInTeam = new(403, "You can only assign to operators in your own team.");
    public static readonly Error OperatorAlreadyAssigned = new(409, "Operator is already assigned to this team.");
}