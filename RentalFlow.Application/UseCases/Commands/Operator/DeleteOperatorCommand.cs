namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed record DeleteOperatorCommand(Guid Id) : ICommand<Result>;

