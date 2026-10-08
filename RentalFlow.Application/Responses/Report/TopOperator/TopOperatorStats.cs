namespace RentalFlow.Application.Responses.Report.TopOperator;

public sealed record TopOperatorStats
{
    [Description("Total de propostas criadas.")]
    public int Total { get; init; }

    [Description("Total de propostas aprovadas.")]
    public int Approved { get; init; }

    [Description("Total de propostas pendentes.")]
    public int Pending { get; init; }

    [Description("Total de propostas rejeitadas.")]
    public int Rejected { get; init; }

    [Description("Valor total das propostas criadas.")]
    public decimal TotalAmount { get; init; }

    [Description("Valor médio por proposta.")]
    public decimal AverageTicket { get; init; }

    [Description("Taxa de conversão (% de propostas aprovadas).")]
    public double ConversionRate { get; init; }
}
