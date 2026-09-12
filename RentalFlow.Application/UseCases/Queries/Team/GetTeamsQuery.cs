using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Team;

public record GetTeamsQuery(GetTeamRequest Request) : IQuery<Result<PagedResult<GetTeamResponse>>>;
