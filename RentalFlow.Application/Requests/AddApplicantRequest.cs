namespace RentalFlow.Application.Requests;

public record AddApplicantRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public decimal MonthlyIncome { get; init; }
}
