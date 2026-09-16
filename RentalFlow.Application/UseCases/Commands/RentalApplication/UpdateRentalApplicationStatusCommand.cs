namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed record UpdateRentalApplicationStatusCommand(Guid Id, UpdateRentalApplicationStatusRequest Request) : ICommand<Result>;
