using LiteBus.Commands.Abstractions;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record DeleteRentalApplicationCommand(Guid Id) : ICommand<Result>;
