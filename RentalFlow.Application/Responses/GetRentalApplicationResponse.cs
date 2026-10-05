namespace RentalFlow.Application.Responses;

public sealed record GetRentalApplicationResponse : BaseResponse
{
    [Description("O número da proposta da solicitação de aluguel.")]
    public string ProposalNumber { get; init; } = string.Empty;

    [Description("O valor financiado para a solicitação de aluguel.")]
    public decimal FinancedAmount { get; init; }

    [Description("O valor total para a solicitação de aluguel.")]
    public decimal TotalAmount { get; init; }

    [Description("O número de parcelas para a solicitação de aluguel.")]
    public int Installments { get; init; }

    [Description("O status da solicitação de aluguel.")]
    public RentalStatus Status { get; init; }

    [Description("A data do contrato de aluguel.")]
    public DateTime? ContractDate { get; init; }

    [Description("O identificador único do candidato.")]
    public Guid ApplicantId { get; init; }

    [Description("O nome do candidato.")]
    public string ApplicantName { get; init; } = string.Empty;

    [Description("O CPF do candidato.")]
    public string ApplicantCpf { get; init; } = string.Empty;

    [Description("O identificador único da propriedade.")]
    public Guid PropertyId { get; init; }

    [Description("O endereço da propriedade.")]
    public string PropertyAddress { get; init; } = string.Empty;

    [Description("O preço de aluguel da propriedade.")]
    public decimal PropertyRentPrice { get; init; }

    [Description("O identificador único do operador.")]
    public Guid OperatorId { get; init; }

    [Description("O nome do operador.")]
    public string OperatorName { get; init; } = string.Empty;
}