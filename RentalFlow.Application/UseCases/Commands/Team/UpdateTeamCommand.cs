namespace RentalFlow.Application.UseCases.Commands.Team;

public record UpdateTeamCommand(Guid Id, UpdateTeamRequest Request) : ICommand<Result>;
