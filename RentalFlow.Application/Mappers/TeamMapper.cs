using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class TeamMapper
{
    public static TeamEntity ToEntity(this AddTeamRequest request)
    {
        return new TeamEntity(
            name: request.Name,
            description: request.Description);
    }

    public static TeamEntity UpdateFrom(this TeamEntity entity, UpdateTeamRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            entity.SetName(request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            entity.SetDescription(request.Description);
        }

        if (request.IsActive.HasValue)
        {
            entity.SetIsActive(request.IsActive.Value);
        }

        return entity.Touch();
    }

    public static GetTeamResponse ToResponse(this TeamEntity entity)
    {
        return new GetTeamResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
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
