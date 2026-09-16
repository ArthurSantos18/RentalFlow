namespace RentalFlow.Application.UseCases.Commands.Team;

public record DeleteTeamCommand(Guid Id) : ICommand<Result>;
