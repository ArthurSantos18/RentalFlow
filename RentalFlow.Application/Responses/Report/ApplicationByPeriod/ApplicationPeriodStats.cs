namespace RentalFlow.Application.Responses.Report.ApplicationByPeriod;

public sealed record ApplicationPeriodStats
{
    [Description("Total de propostas no período.")]
    public int Total { get; init; }

    [Description("Total de propostas aprovadas.")]
    public int Approved { get; init; }

    [Description("Total de propostas pendentes.")]
    public int Pending { get; init; }

    [Description("Total de propostas rejeitadas.")]
    public int Rejected { get; init; }

    [Description("Valor total das propostas.")]
    public decimal TotalAmount { get; init; }

    [Description("Valor total financiado.")]
    public decimal TotalFinancedAmount { get; init; }

    [Description("Taxa de conversão (% de propostas aprovadas).")]
    public double ConversionRate { get; init; }
}
