namespace RentalFlow.Application.Models.Aggregates;

public sealed record TopOperatorAggregate
{
    public Guid OperatorId { get; init; }
    public int Total { get; init; }
    public int Approved { get; init; }
    public int Pending { get; init; }
    public int Rejected { get; init; }
    public decimal TotalAmount { get; init; }
}
