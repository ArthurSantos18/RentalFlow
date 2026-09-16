namespace RentalFlow.Application.UseCases.Commands.Property;

public record UpdatePropertyCommand(Guid Id, UpdatePropertyRequest Request) : ICommand<Result>;
