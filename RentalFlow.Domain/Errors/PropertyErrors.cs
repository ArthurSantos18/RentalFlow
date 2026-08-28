using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class PropertyErrors
{
    public static Error PropertyNotFound = new(404, "Property not found.");
}
