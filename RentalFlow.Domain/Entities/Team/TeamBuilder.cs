using RentalFlow.Domain.Entities.Operator;

namespace RentalFlow.Domain.Entities.Team;

public sealed class TeamBuilder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<OperatorEntity> Operators { get; set; } = [];

    public static TeamBuilder Create() => new();

    public TeamBuilder WithId(Guid id)
    {
        Id = id;
        return this;
    }

    public TeamBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public TeamBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    public TeamBuilder WithIsActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }

    public TeamEntity Build()
    {
        return new TeamEntity(
            Id,
            Name,
            Description,
            IsActive,
            Operators
        );
    }
}
