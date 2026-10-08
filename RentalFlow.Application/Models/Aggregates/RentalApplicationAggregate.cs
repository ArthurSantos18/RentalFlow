namespace RentalFlow.Application.Models.Aggregates;

public sealed record RentalApplicationAggregate
{
    public int Total { get; init; }
    public int Draft { get; init; }
    public int Pending { get; init; }
    public int Approved { get; init; }
    public int Rejected { get; init; }
    public decimal TotalFinancedAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal AverageTicket => Total == 0 ? 0 : Math.Round(TotalAmount / Total, 2);
}