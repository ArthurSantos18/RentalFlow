namespace RentalFlow.Application.UseCases.Commands.Property;

public record DeletePropertyCommand(Guid Id) : ICommand<Result>;
