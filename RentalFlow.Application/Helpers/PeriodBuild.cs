namespace RentalFlow.Application.Helpers;

public static class PeriodBuild
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    public static string BuildKey(DateTime periodStart, PeriodGroup groupBy)
    {
        return groupBy switch
        {
            PeriodGroup.Day => periodStart.ToString("yyyy-MM-dd"),
            PeriodGroup.Week => $"{ISOWeek.GetYear(periodStart):D4}-W{ISOWeek.GetWeekOfYear(periodStart):D2}",
            PeriodGroup.Month => periodStart.ToString("yyyy-MM"),
            PeriodGroup.Year => periodStart.ToString("yyyy"),
            _ => periodStart.ToString("yyyy-MM-dd"),
        };
    }

    public static string BuildLabel(DateTime periodStart, PeriodGroup groupBy)
    {
        return groupBy switch
        {
            PeriodGroup.Day => periodStart.ToString("dd 'de' MMMM 'de' yyyy", PtBr),
            PeriodGroup.Week => $"Semana {ISOWeek.GetWeekOfYear(periodStart)} de {ISOWeek.GetYear(periodStart)}",
            PeriodGroup.Month => periodStart.ToString("MMMM 'de' yyyy", PtBr),
            PeriodGroup.Year => periodStart.ToString("yyyy", PtBr),
            _ => periodStart.ToString("yyyy-MM-dd", PtBr),
        };
    }
}