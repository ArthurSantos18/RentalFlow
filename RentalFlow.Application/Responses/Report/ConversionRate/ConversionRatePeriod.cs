namespace RentalFlow.Application.Responses.Report.ConversionRate;

public sealed record ConversionRatePeriod
{
    [Description("Data inicial do período (UTC).")]
    public DateTime From { get; init; }

    [Description("Data final do período (UTC).")]
    public DateTime To { get; init; }

    [Description("Total de propostas.")]
    public int Total { get; init; }

    [Description("Total de propostas em rascunho.")]
    public int Draft { get; init; }

    [Description("Total de propostas pendentes.")]
    public int Pending { get; init; }

    [Description("Total de propostas aprovadas.")]
    public int Approved { get; init; }

    [Description("Total de propostas rejeitadas.")]
    public int Rejected { get; init; }

    [Description("Taxa de conversão (% de propostas aprovadas).")]
    public double ConversionRate { get; init; }
}
