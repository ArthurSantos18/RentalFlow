namespace RentalFlow.Application.Requests.RentalApplication;

public sealed record UpdateRentalApplicationStatusRequest
{
    public RentalStatus RentalStatus { get; init; }
}
