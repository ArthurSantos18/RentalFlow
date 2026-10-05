namespace RentalFlow.Application.Requests.RentalApplication;

public sealed record AddRentalApplicationRequest
{
    [Description("O ID do candidato.")]
    public Guid ApplicantId { get; init; }

    [Description("O ID da propriedade.")]
    public Guid PropertyId { get; init; }

    [Description("O ID do operador.")]
    public Guid? OperatorId { get; init; }

    [Description("O valor financiado para a solicitação de aluguel.")]
    public decimal FinancedAmount { get; init; }

    [Description("O valor total para a solicitação de aluguel.")]
    public decimal TotalAmount { get; init; }

    [Description("O número de parcelas para a solicitação de aluguel.")]
    public int Installments { get; init; }

    [Description("A data do contrato de aluguel.")]
    public DateTime? ContractDate { get; init; }
}