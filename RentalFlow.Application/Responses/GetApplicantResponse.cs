namespace RentalFlow.Application.Responses;

public record GetApplicantResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public decimal MonthlyIncome { get; init; }
    public bool IsActive { get; init; }
}
