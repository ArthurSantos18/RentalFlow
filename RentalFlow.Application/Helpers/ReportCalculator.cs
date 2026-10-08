namespace RentalFlow.Application.Helpers;

public static class ReportCalculator
{
    public static double ConversionRate(int approved, int total)
    {
        return total == 0 ? 0 : Math.Round((double)approved / total * 100, 2);
    }

    public static decimal AverageTicket(decimal totalAmount, int total)
    {
        return total == 0 ? 0 : Math.Round(totalAmount / total, 2);
    }

    public static (DateTime From, DateTime To) CalculatePreviousPeriod(DateTime from, DateTime to)
    {
        var duration = to - from;
        var previousTo = from.AddTicks(-1);
        var previousFrom = previousTo - duration;

        return (previousFrom, previousTo);
    }
}
