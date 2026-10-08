namespace RentalFlow.Application.Responses.Report.ConversionRate;

public sealed record ConversionRateComparison
{
    [Description("Diferença absoluta no total de propostas.")]
    public int TotalDelta { get; init; }

    [Description("Diferença absoluta no total de aprovadas.")]
    public int ApprovedDelta { get; init; }

    [Description("Diferença em pontos percentuais na taxa de conversão.")]
    public double ConversionRateDelta { get; init; }

    [Description("Variação percentual na taxa de conversão.")]
    public double ConversionRateChangePercentage { get; init; }
}
