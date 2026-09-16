namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed record UpdateTeamCommand(Guid Id, UpdateTeamRequest Request) : ICommand<Result>;
