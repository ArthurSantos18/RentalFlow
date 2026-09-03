using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record UpdateRentalApplicationCommand(Guid Id, UpdateRentalApplicationRequest Request) : ICommand<Result>;
