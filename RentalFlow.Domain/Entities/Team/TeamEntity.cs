using RentalFlow.Domain.Entities.Operator;

namespace RentalFlow.Domain.Entities.Team;

public sealed class TeamEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public List<OperatorEntity> Operators { get; private set; } = [];

    public TeamEntity(
        Guid id,
        string name,
        string? description,
        bool isActive,
        List<OperatorEntity> operators)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        Operators = operators ?? [];
    }

    public static TeamEntity Empty { get; } = new TeamEntity
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Description = null,
        IsActive = false,
        Operators = []
    };

    private TeamEntity() { }

    public TeamEntity SetId(Guid id)
    {
        Id = id;
        return this;
    }

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

    public TeamEntity SetIsActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }

    public TeamEntity SetOperators(List<OperatorEntity> operators)
    {
        Operators = operators ?? [];
        return this;
    }

    public TeamEntity AddOperator(OperatorEntity operatorEntity)
    {
        Operators.Add(operatorEntity);
        return this;
    }

    public TeamEntity RemoveOperator(OperatorEntity operatorEntity)
    {
        Operators.Remove(operatorEntity);
        return this;
    }

    public TeamBuilder ToBuilder() => new()
    {
        Id = Id,
        Name = Name,
        Description = Description,
        IsActive = IsActive,
        Operators = Operators
    };
}
