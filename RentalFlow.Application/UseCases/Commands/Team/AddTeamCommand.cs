namespace RentalFlow.Application.UseCases.Commands.Team;

public sealed record AddTeamCommand(AddTeamRequest Request) : ICommand<Result<Guid>>;
