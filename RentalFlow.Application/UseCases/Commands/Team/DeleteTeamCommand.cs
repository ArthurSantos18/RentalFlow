using LiteBus.Commands.Abstractions;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Team;

public record DeleteTeamCommand(Guid Id) : ICommand<Result>;
