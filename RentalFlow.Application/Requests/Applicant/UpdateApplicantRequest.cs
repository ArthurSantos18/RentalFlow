namespace RentalFlow.Application.Requests.Applicant;

public sealed record UpdateApplicantRequest
{
    [Description("O novo nome completo do inquilino.")]
    public string? FullName { get; init; }

    [Description("O novo email do inquilino.")]
    public string? Email { get; init; }

    [Description("O novo número de telefone do inquilino.")]
    public string? Phone { get; init; }

    [Description("A nova renda mensal do inquilino.")]
    public decimal? MonthlyIncome { get; init; }

    [Description("O novo status ativo para o inquilino.")]
    public bool? IsActive { get; init; }
};