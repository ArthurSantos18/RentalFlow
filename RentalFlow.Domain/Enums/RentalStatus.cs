using System.Text.Json.Serialization;

namespace RentalFlow.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RentalStatus
{
    None = 0,
    Draft = 1,
    Pending = 2,
    Approved = 3,
    Rejected = 4
}
