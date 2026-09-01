using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.Operator;

public sealed class OperatorEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public OperatorRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public OperatorEntity(Guid id, string name, string email, OperatorRole role, bool isActive, List<RentalApplicationEntity> applications)
    {
        Id = id;
        Name = name;
        Email = email;
        Role = role;
        IsActive = isActive;
        Applications = applications ?? [];
    }

    public static OperatorEntity Empty { get; } = new OperatorEntity
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Email = string.Empty,
        Applications = [],
        IsActive = false,
        Role = OperatorRole.None,
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

    public OperatorEntity SetRole(OperatorRole role)
    {
        Role = role;
        return this;
    }

    public OperatorEntity SetIsActive(bool isActive)
    {
        IsActive = isActive;
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

    public void Update(OperatorUpdate update)
    {
        if (update.Name is not null)
        {
            SetName(update.Name);
        }
            
        if (update.Email is not null)
        {
            SetEmail(update.Email);
        }
            
        if (update.Role.HasValue)
        {
            SetRole(update.Role.Value);
        }
    }

    public OperatorBuilder ToBuilder() => new()
    {
        Id = Id,
        Name = Name,
        Email = Email,
        Role = Role,
        IsActive = IsActive,
        Applications = Applications
    };
}
