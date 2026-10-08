namespace RentalFlow.Application.Models.Aggregates;

public sealed record TeamAggregate
{
    public int Total { get; init; }
    public int Active { get; init; }
    public int Inactive => Total - Active;
}
