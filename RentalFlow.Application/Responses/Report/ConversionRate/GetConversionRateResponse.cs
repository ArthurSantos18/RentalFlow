namespace RentalFlow.Application.Responses.Report.ConversionRate;

public sealed record GetConversionRateResponse
{
    [Description("Data inicial do período (UTC).")]
    public DateTime From { get; init; }

    [Description("Data final do período (UTC).")]
    public DateTime To { get; init; }

    [Description("Estatísticas do período atual.")]
    public ConversionRatePeriod Current { get; init; } = new();

    [Description("Estatísticas do período anterior (se solicitada comparação).")]
    public ConversionRatePeriod? Previous { get; init; }

    [Description("Comparação entre os dois períodos (se solicitada).")]
    public ConversionRateComparison? Comparison { get; init; }
}
