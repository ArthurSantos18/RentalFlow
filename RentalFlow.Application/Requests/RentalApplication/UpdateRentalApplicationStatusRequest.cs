namespace RentalFlow.Application.Requests.RentalApplication;

public sealed record UpdateRentalApplicationStatusRequest
{
    [Description("The new status for the rental application.")]
    public RentalStatus RentalStatus { get; init; }
}