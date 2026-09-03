using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Domain.Errors;

public static class ApplicantErrors
{
    public static Error ApplicantDoesExist = new(409, "Applicant already exists with this CPF.");
    public static Error ApplicantNotFound = new(404, "Applicant not found.");
    public static Error ApplicantIsInactive = new(409, "Applicant is inactive.");
}
