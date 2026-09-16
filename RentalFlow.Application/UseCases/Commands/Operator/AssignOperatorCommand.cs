namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed record AssignOperatorCommand(Guid OperatorId, Guid TeamId) : ICommand<Result>;
