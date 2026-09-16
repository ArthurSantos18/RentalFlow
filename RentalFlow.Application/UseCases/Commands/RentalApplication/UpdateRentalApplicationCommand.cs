namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record UpdateRentalApplicationCommand(Guid Id, UpdateRentalApplicationRequest Request) : ICommand<Result>;
