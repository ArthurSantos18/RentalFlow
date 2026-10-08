namespace RentalFlow.Application.Responses.Report.ApplicationByPeriod;

public sealed record ApplicationPeriodItem
{
    [Description("Data de início do período (UTC).")]
    public DateTime PeriodStart { get; init; }

    [Description("Chave do período no formato `yyyy-MM-dd`, `yyyy-Www`, `yyyy-MM` ou `yyyy`.")]
    public string PeriodKey { get; init; } = string.Empty;

    [Description("Rótulo legível do período.")]
    public string PeriodLabel { get; init; } = string.Empty;

    [Description("Estatísticas das propostas no período.")]
    public ApplicationPeriodStats Applications { get; init; } = new();
}
