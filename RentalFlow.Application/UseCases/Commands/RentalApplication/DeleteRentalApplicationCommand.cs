namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed record DeleteRentalApplicationCommand(Guid Id) : ICommand<Result>;
