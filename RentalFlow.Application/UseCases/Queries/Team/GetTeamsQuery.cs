namespace RentalFlow.Application.UseCases.Queries.Team;

public record GetTeamsQuery(GetTeamRequest Request) : IQuery<Result<PagedResult<GetTeamResponse>>>;
