namespace RentalFlow.Application.Models.Aggregates;

public sealed record TopPropertyAggregate
{
    public Guid PropertyId { get; init; }
    public int Total { get; init; }
    public int Approved { get; init; }
    public int Pending { get; init; }
    public int Rejected { get; init; }
}
