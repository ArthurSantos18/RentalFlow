using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Property;

public record UpdatePropertyCommand(Guid Id, UpdatePropertyRequest Request) : ICommand<Result>;
