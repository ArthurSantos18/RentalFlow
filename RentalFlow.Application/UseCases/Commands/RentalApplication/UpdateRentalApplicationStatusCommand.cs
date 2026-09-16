namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record UpdateRentalApplicationStatusCommand(Guid Id, UpdateRentalApplicationStatusRequest Request) : ICommand<Result>;
