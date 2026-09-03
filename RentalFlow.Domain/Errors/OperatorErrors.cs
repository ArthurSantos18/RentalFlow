using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class OperatorErrors
{
    public static Error OperatorNotFound = new(404, "Operator not found.");
    public static Error OperatorIsInactive = new(409, "Operator is inactive.");
}