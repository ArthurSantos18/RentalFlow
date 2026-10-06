namespace RentalFlow.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreAuditAttribute : Attribute { }
