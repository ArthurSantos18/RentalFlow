namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record DeleteRentalApplicationCommand(Guid Id) : ICommand<Result>;
