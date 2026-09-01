using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.Operator;
public sealed class OperatorBuilder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public OperatorRole Role { get; set; }
    public bool IsActive { get; set; }
    public List<RentalApplicationEntity> Applications { get; set; } = [];

    public static OperatorBuilder Create() => new();

    public OperatorBuilder WithId(Guid id) { Id = id; return this; }

    public OperatorBuilder WithName(string name) { Name = name; return this; }

    public OperatorBuilder WithEmail(string email) { Email = email; return this; }

    public OperatorBuilder WithRole(OperatorRole role) { Role = role; return this; }

    public OperatorBuilder WithIsActive(bool isActive) { IsActive = isActive; return this; }

    public OperatorBuilder WithApplications(List<RentalApplicationEntity> applications)
    {
        Applications = applications ?? [];
        return this;
    }

    public OperatorBuilder AddApplication(RentalApplicationEntity application)
    {
        Applications.Add(application);
        return this;
    }

    public OperatorEntity Build()
    {
        return new OperatorEntity(
            Id,
            Name,
            Email,
            Role,
            IsActive,
            Applications
        );
    }
}