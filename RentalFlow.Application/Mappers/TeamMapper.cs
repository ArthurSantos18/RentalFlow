using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class TeamMapper
{
    public static TeamEntity ToEntity(this AddTeamRequest request)
    {
        return new TeamBuilder()
            .WithId(Guid.NewGuid())
            .WithName(request.Name)
            .WithDescription(request.Description)
            .WithIsActive(true)
            .Build();
    }

    public static TeamEntity UpdateEntity(this UpdateTeamRequest request, TeamEntity entity)
    {
        return entity
            .SetName(request.Name ?? entity.Name)
            .SetDescription(request.Description ?? entity.Description);
    }

    public static GetTeamResponse ToResponse(this TeamEntity entity)
    {
        return new GetTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public static PagedResult<GetTeamResponse> ToResponse(this PagedResult<TeamEntity> pagedResult)
    {
        return new PagedResult<GetTeamResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(t => t.ToResponse())
        };
    }
}
