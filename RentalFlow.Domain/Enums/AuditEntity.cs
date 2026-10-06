namespace RentalFlow.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuditEntity
{
    None = 0,
    CustomerEntity = 1,
    PropertyEntity = 2,
    RentalApplicationEntity = 3,
    OperatorEntity = 4,
    ApplicantEntity = 5
}
