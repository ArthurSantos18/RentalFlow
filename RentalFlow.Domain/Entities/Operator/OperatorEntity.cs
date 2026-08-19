using RentalFlow.Domain.Entities.RentalApplication;

namespace RentalFlow.Domain.Entities.Operator;

public sealed class OperatorEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public OperatorEntity(Guid id, string name, string email, string role, List<RentalApplicationEntity> applications)
    {
        Id = id;
        Name = name;
        Email = email;
        Role = role;
        Applications = applications ?? [];
    }

    public static OperatorEntity Empty { get; } = new OperatorEntity
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Email = string.Empty,
        Applications = [],
        Role = string.Empty
    };

    private OperatorEntity() { }

    public OperatorEntity SetId(Guid id)
    {
        Id = id;
        return this;
    }

    public OperatorEntity SetName(string name)
    {
        Name = name;
        return this;
    }

    public OperatorEntity SetEmail(string email)
    {
        Email = email;
        return this;
    }

    public OperatorEntity SetRole(string role)
    {
        Role = role;
        return this;
    }

    public OperatorEntity SetApplications(List<RentalApplicationEntity> applications)
    {
        Applications = applications ?? [];
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

    public OperatorBuilder ToBuilder() => new()
    {
        Id = Id,
        Name = Name,
        Email = Email,
        Role = Role,
        Applications = Applications
    };
}
