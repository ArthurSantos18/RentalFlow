namespace RentalFlow.Infrastructure.Helpers;

public static class PeriodHelper
{
    public static Expression<Func<RentalApplicationEntity, int>> GetGroupKey(DateTime referenceDate, PeriodGroup groupBy)
    {
        return groupBy switch
        {
            PeriodGroup.Day => x =>
                EF.Functions.DateDiffDay(referenceDate, x.CreatedAt),

            PeriodGroup.Week => x =>
                EF.Functions.DateDiffDay(referenceDate, x.CreatedAt) / 7,

            PeriodGroup.Month => x =>
                EF.Functions.DateDiffMonth(referenceDate, x.CreatedAt),

            PeriodGroup.Year => x =>
                EF.Functions.DateDiffYear(referenceDate, x.CreatedAt),

            _ => throw new ArgumentOutOfRangeException(nameof(groupBy))
        };
    }

    public static DateTime NormalizeReferenceDate(DateTime from, PeriodGroup groupBy)
    {
        return groupBy switch
        {
            PeriodGroup.Day or PeriodGroup.Week => from.Date,
            PeriodGroup.Month => new DateTime(from.Year, from.Month, 1, 0, 0, 0, DateTimeKind.Utc),
            PeriodGroup.Year => new DateTime(from.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            _ => from.Date,
        };
    }

    public static DateTime GetPeriodStart(DateTime reference, PeriodGroup groupBy, int periodIndex)
    {
        return groupBy switch
        {
            PeriodGroup.Day => reference.AddDays(periodIndex),
            PeriodGroup.Week => reference.AddDays(periodIndex * 7),
            PeriodGroup.Month => reference.AddMonths(periodIndex),
            PeriodGroup.Year => reference.AddYears(periodIndex),
            _ => reference,
        };
    }
}