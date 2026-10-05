namespace RentalFlow.Application.Requests.Applicant;

public sealed record AddApplicantRequest
{
    [Description("O nome completo do inquilino.")]
    public string FullName { get; init; } = string.Empty;

    [Description("O CPF do inquilino.")]
    public string Cpf { get; init; } = string.Empty;

    [Description("O email do inquilino.")]
    public string Email { get; init; } = string.Empty;

    [Description("O número de telefone do inquilino.")]
    public string Phone { get; init; } = string.Empty;

    [Description("A renda mensal do inquilino.")]
    public decimal MonthlyIncome { get; init; }
}