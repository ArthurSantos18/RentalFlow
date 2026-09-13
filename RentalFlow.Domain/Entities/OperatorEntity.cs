using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities;

public sealed class OperatorEntity : BaseEntity<OperatorEntity>
{
    public Guid TeamId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public OperatorRole Role { get; private set; }
    public UserEntity User { get; private set; } = null!;
    public TeamEntity Team { get; private set; } = null!;
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public OperatorEntity(
        string name,
        OperatorRole role,
        TeamEntity team)
    {
        Name = name;
        TeamId = team.Id;
        Role = role;
        Team = team;
    }

    private OperatorEntity() { }

    public OperatorEntity SetName(string name)
    {
        Name = name;
        return this;
    }

    public OperatorEntity SetRole(OperatorRole role)
    {
        Role = role;
        return this;
    }

    public OperatorEntity SetTeam(TeamEntity team)
    {
        TeamId = team.Id;
        Team = team;
        return this;
    }

    public OperatorEntity AddApplication(RentalApplicationEntity application)
    {
        Applications.Add(application);
        return this;
    }

    public OperatorEntity RemoveApplication(RentalApplicationEntity application)
    {
        Applications.Remove(application);
        return this;
    }
}
