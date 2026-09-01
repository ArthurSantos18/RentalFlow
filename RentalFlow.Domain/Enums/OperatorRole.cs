using System.Text.Json.Serialization;

namespace RentalFlow.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OperatorRole
{
    None = 0,
    Broker = 1,
    Manager = 2,
    Administrator = 3
}
