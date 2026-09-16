namespace RentalFlow.Application.UseCases.Commands.Property;

public record AddPropertyCommand(AddPropertyRequest Request) : ICommand<Result<Guid>>;