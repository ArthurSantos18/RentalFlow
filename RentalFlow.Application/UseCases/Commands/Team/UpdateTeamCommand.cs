using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Team;

public record UpdateTeamCommand(Guid Id, UpdateTeamRequest Request) : ICommand<Result>;
