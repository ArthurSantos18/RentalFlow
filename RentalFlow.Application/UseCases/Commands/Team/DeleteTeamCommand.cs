namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed record DeleteTeamCommand(Guid Id) : ICommand<Result>;
