namespace RentalFlow.Domain.Entities.Applicant;

public record ApplicantUpdate(
    string? FullName,
    string? Cpf,
    string? Email,
    string? Phone,
    decimal? MonthlyIncome);