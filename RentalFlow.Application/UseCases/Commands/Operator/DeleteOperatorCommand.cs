using LiteBus.Commands.Abstractions;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Operator;

public record DeleteOperatorCommand(Guid Id) : ICommand<Result>;

