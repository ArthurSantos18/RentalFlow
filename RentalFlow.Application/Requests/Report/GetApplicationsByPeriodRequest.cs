namespace RentalFlow.Application.Requests.Report;

public sealed class GetApplicationsByPeriodRequest
{
    [Description("Data de início para filtrar as propriedades.")]
    public DateTime From { get; init; }

    [Description("Data de término para filtrar as propriedades.")]
    public DateTime To { get; init; }

    [Description("Define o agrupamento das aplicações por dia, semana, mês ou ano.")]
    public PeriodGroup GroupBy { get; init; } = PeriodGroup.Month;
}