namespace RentalFlow.Application.Responses.Report.ApplicationByPeriod;

public sealed record GetApplicationsByPeriodResponse
{
    [Description("Data inicial do período (UTC).")]
    public DateTime From { get; init; }

    [Description("Data final do período (UTC).")]
    public DateTime To { get; init; }

    [Description("Agrupamento aplicado aos resultados.")]
    public PeriodGroup GroupBy { get; init; }

    [Description("Lista de períodos ordenados cronologicamente.")]
    public IReadOnlyList<ApplicationPeriodItem> Items { get; init; } = [];
}
