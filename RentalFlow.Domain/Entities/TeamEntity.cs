namespace RentalFlow.Domain.Entities;

public sealed class TeamEntity : BaseEntity<TeamEntity>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public List<OperatorEntity> Operators { get; private set; } = [];

    public TeamEntity(
        string name,
        string description)
    {
        Name = name;
        Description = description;
    }

    private TeamEntity() { }

    public TeamEntity SetName(string name)
    {
        Name = name;
        return this;
    }

    public TeamEntity SetDescription(string description)
    {
        Description = description;
        return this;
    }

    public TeamEntity AddOperator(OperatorEntity @operator)
    {
        Operators.Add(@operator);
        return this;
    }

    public TeamEntity RemoveOperator(OperatorEntity @operator)
    {
        Operators.Remove(@operator);
        return this;
    }
}
