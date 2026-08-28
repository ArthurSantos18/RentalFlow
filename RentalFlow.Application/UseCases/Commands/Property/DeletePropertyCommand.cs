using LiteBus.Commands.Abstractions;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Property;

public record DeletePropertyCommand(Guid Id) : ICommand<Result>;
