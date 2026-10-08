namespace RentalFlow.Application.Models.Aggregates;

public sealed record OperatorAggregate
{
    public int Total { get; init; }
    public int Active { get; init; }
    public int Broker { get; init; }
    public int Manager { get; init; }
    public int Administrator { get; init; }
    public int Inactive => Total - Active;
}
