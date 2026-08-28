using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Property;

public record AddPropertyCommand(AddPropertyRequest Request) : ICommand<Result<Guid>>;