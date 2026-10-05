namespace RentalFlow.Application.Responses;

public sealed record GetApplicantResponse : BaseResponse
{
    [Description("O nome completo do candidato.")]
    public string FullName { get; init; } = string.Empty;

    [Description("O CPF do candidato.")]
    public string Cpf { get; init; } = string.Empty;

    [Description("O email do candidato.")]
    public string Email { get; init; } = string.Empty;

    [Description("O número de telefone do candidato.")]
    public string? Phone { get; init; }

    [Description("A renda mensal do candidato.")]
    public decimal MonthlyIncome { get; init; }
}