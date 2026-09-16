namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed record AddPropertyCommand(AddPropertyRequest Request) : ICommand<Result<Guid>>;