namespace RentalFlow.Application.UseCases.Commands.Team;

public record AddTeamCommand(AddTeamRequest Request) : ICommand<Result<Guid>>;
