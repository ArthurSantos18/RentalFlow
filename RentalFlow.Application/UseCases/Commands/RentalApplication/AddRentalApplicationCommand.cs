namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record AddRentalApplicationCommand(AddRentalApplicationRequest Request) : ICommand<Result<Guid>>;
