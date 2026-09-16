namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed record UpdateRentalApplicationCommand(Guid Id, UpdateRentalApplicationRequest Request) : ICommand<Result>;
