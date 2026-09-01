using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public record UpdateOperatorCommand(Guid Id, UpdateOperatorRequest Request) : ICommand<Result>;

