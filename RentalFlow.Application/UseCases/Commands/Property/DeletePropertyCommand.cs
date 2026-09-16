namespace RentalFlow.Application.UseCases.Commands.Property;

public sealed record DeletePropertyCommand(Guid Id) : ICommand<Result>;
