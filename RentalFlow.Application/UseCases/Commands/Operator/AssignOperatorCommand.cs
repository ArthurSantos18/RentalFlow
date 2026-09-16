namespace RentalFlow.Application.UseCases.Commands.Operator;

public record AssignOperatorCommand(Guid OperatorId, Guid TeamId) : ICommand<Result>;
