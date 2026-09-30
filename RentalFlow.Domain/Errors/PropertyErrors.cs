namespace RentalFlow.Domain.Errors;

public static class PropertyErrors
{
    public static readonly Error PropertyNotFound = new(404, "Property not found.");
    public static readonly Error PropertyIsInactive = new(409, "Property is inactive.");
    public static readonly Error PropertyNotAvailable = new(409, "Property is not available for rent.");
    public static readonly Error PropertyHasApplications = new(409, "Cannot delete property because it has associated rental applications.");
}
