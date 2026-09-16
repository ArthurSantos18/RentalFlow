namespace RentalFlow.Application.UseCases.Commands.Operator;

public record DeleteOperatorCommand(Guid Id) : ICommand<Result>;

