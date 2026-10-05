namespace RentalFlow.Application.Requests.RentalApplication;

public sealed record UpdateRentalApplicationStatusRequest
{
    [Description("O novo status para a solicitação de aluguel.")]
    public RentalStatus RentalStatus { get; init; }
}