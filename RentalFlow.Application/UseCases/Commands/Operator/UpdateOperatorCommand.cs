namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed record UpdateOperatorCommand(Guid Id, UpdateOperatorRequest Request) : ICommand<Result>;

