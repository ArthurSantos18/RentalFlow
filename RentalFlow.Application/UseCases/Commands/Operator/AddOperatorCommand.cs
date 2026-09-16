namespace RentalFlow.Application.UseCases.Commands.Operator;

public record AddOperatorCommand(AddOperatorRequest Request) : ICommand<Result<Guid>>;
