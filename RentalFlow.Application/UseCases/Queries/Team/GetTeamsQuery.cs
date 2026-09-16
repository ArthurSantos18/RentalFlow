namespace RentalFlow.Application.UseCases.Queries.Team;

public sealed record GetTeamsQuery(GetTeamRequest Request) : IQuery<Result<PagedResult<GetTeamResponse>>>;
