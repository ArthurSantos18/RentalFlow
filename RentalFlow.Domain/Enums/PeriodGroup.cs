namespace RentalFlow.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PeriodGroup
{
    None = 0,
    Day = 1,
    Week = 2,
    Month = 3,
    Year = 4
}
