namespace RentalFlow.Application.Models.Aggregates;

public sealed record ConversionRateAggregate
{
    public int Total { get; init; }
    public int Draft { get; init; }
    public int Pending { get; init; }
    public int Approved { get; init; }
    public int Rejected { get; init; }
}
