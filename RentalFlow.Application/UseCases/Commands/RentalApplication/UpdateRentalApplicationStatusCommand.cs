using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record UpdateRentalApplicationStatusCommand(Guid Id, UpdateRentalApplicationStatusRequest Request) : ICommand<Result>;
