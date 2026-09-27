namespace RentalFlow.Domain.Errors;

public static class ApplicantErrors
{
    public static readonly Error ApplicantDoesExist = new(409, "Applicant already exists with this CPF.");
    public static readonly Error ApplicantNotFound = new(404, "Applicant not found.");
    public static readonly Error ApplicantIsInactive = new(409, "Applicant is inactive.");
}
