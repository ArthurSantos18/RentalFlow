namespace RentalFlow.Application.Helpers;

public static class ConversionRateCalculator
{
    public static double Calculate(int approved, int total)
    {
        return total == 0
            ? 0
            : Math.Round((double)approved / total * 100, 2);
    }
}
