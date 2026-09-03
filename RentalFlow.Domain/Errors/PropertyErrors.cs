using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class PropertyErrors
{
    public static Error PropertyNotFound = new(404, "Property not found.");
    public static Error PropertyIsInactive = new(409, "Property is inactive.");
    public static Error PropertyNotAvailable = new(409, "Property is not available for rent.");
}
