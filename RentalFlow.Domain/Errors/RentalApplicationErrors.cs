namespace RentalFlow.Domain.Errors;

public static class RentalApplicationErrors
{
    public static readonly Error RentalApplicationNotFound = new(404, "Rental application not found.");
    public static readonly Error InvalidStatusTransition = new(409, "Invalid status transition for rental application.");
    public static readonly Error RentalApplicationCannotBeEdited = new(409, "This rental application cannot be edited after the application has been approved or rejected.");
    public static readonly Error OperatorAlreadyAssigned = new(409, "The operator is already assigned to this rental application.");
    public static readonly Error PropertyAlreadyAssigned = new(409, "The property is already assigned to this rental application.");
    public static readonly Error ApplicantAlreadyAssigned = new(409, "The applicant is already assigned to this rental application.");
}