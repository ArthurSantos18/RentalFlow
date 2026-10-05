namespace RentalFlow.Application.Requests.RentalApplication;

public sealed record UpdateRentalApplicationRequest
{
    [Description("O novo número de parcelas para a solicitação de aluguel.")]
    public int? Installments { get; init; }

    [Description("O novo valor financiado para a solicitação de aluguel.")]
    public decimal? FinancedAmount { get; init; }

    [Description("O novo valor total para a solicitação de aluguel.")]
    public decimal? TotalAmount { get; init; }

    [Description("A nova data do contrato de aluguel.")]
    public DateTime? ContractDate { get; init; }

    [Description("O novo ID do candidato.")]
    public Guid? ApplicantId { get; init; }

    [Description("O novo ID do operador.")]
    public Guid? OperatorId { get; init; }

    [Description("O novo ID da propriedade.")]
    public Guid? PropertyId { get; init; }
}