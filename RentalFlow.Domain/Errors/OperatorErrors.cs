using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class OperatorErrors
{
    public static Error OperatorNotFound = new(404, "Operator not found.");
    public static Error OperatorIsInactive = new(409, "Operator is inactive.");
    public static Error OperatorAlreadyAssigned = new(409, "Operator is already assigned to this team.");
}