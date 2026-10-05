namespace RentalFlow.Domain.Errors;

public static class ApplicantErrors
{
    public static readonly Error ApplicantAlreadyExists = new(409, "Applicant already exists with this CPF.");
    public static readonly Error ApplicantNotFound = new(404, "Applicant not found.");
    public static readonly Error ApplicantHasApplications = new(409, "Cannot delete applicant because they have associated rental applications.");
    public static readonly Error ApplicantInactive = new(409, "Applicant is inactive.");
}