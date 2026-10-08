namespace RentalFlow.Application.Responses.Report.TopProperty;

public sealed record TopPropertyStats
{
    [Description("Total de propostas recebidas.")]
    public int Total { get; init; }

    [Description("Total de propostas aprovadas.")]
    public int Approved { get; init; }

    [Description("Total de propostas pendentes.")]
    public int Pending { get; init; }

    [Description("Total de propostas rejeitadas.")]
    public int Rejected { get; init; }

    [Description("Taxa de conversão (% de propostas aprovadas).")]
    public double ConversionRate { get; init; }
}