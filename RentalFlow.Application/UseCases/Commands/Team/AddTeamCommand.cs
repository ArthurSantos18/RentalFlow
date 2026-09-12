using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Team;

public record AddTeamCommand(AddTeamRequest Request) : ICommand<Result<Guid>>;
