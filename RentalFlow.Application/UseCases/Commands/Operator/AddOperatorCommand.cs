namespace RentalFlow.Application.UseCases.Commands.Operator;

public sealed record AddOperatorCommand(AddOperatorRequest Request) : ICommand<Result<Guid>>;
