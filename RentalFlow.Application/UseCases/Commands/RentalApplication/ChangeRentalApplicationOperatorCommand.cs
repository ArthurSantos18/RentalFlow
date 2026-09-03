using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record ChangeRentalApplicationOperatorCommand(Guid Id, ChangeRentalApplicationOperatorRequest Request) : ICommand<Result>;
