namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed record UpdatePropertyCommand(Guid Id, UpdatePropertyRequest Request) : ICommand<Result>;
