namespace RentalFlow.Application.Requests;

public record UpdateApplicantRequest
{
    public string? FullName { get; init; }
    public string? Cpf { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public decimal? MonthlyIncome { get; init; }
};
