using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class RentalApplicationErrors
{
    public static Error RentalApplicationNotFound = new(404, "Rental application not found.");
    public static Error RentalApplicationOperatorChangeNotAllowed = new(409, "Cannot change operator after the application has been approved or rejected.");
    public static Error RentalApplicationPropertyChangeNotAllowed = new(409, "Cannot change property after the application has been reviewed or approved.");
    public static Error RentalApplicationApplicantChangeNotAllowed = new(409, "Cannot change applicant after the application has been reviewed or approved.");
    public static Error OperatorAlreadyAssigned = new(409, "The operator is already assigned to this rental application.");
    public static Error PropertyAlreadyAssigned = new(409, "The property is already assigned to this rental application.");
    public static Error ApplicantAlreadyAssigned = new(409, "The applicant is already assigned to this rental application.");
}
