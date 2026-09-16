namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public sealed record AddRentalApplicationCommand(AddRentalApplicationRequest Request) : ICommand<Result<Guid>>;
