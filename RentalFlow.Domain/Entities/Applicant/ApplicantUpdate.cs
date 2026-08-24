namespace RentalFlow.Domain.Entities.Applicant;

public sealed record ApplicantUpdate(
    string? FullName,
    string? Cpf,
    string? Email,
    string? Phone,
    decimal? MonthlyIncome);