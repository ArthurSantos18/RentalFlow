namespace RentalFlow.Application.UseCases.Commands.Operator;

public record UpdateOperatorCommand(Guid Id, UpdateOperatorRequest Request) : ICommand<Result>;

