namespace RentalFlow.Application.Models.Aggregates;

public sealed record PropertyAggregate
{
    public int Total { get; init; }
    public int Available { get; init; }
    public int Active { get; init; }
    public int Rented => Total - Available;
    public int Inactive => Total - Active;
}