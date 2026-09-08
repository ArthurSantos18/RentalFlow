using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Requests.RentalApplication;

public record UpdateRentalApplicationStatusRequest
{
    public RentalStatus RentalStatus { get; init; }
}
