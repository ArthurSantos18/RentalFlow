namespace RentalFlow.Application.Models;

public abstract record DataScope()
{
    public sealed record Global() : DataScope;

    public sealed record ForOperator(Guid OperatorId) : DataScope;

    public sealed record ForTeam(Guid TeamId) : DataScope;

    public sealed record Deny() : DataScope;
}