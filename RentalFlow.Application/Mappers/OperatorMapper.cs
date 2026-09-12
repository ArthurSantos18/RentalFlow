using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class OperatorMapper
{
    public static OperatorEntity ToEntity(this AddOperatorRequest request, TeamEntity team)
    {
        return new OperatorBuilder()
            .WithId(Guid.NewGuid())
            .WithName(request.Name)
            .WithEmail(request.Email)
            .WithRole(request.Role)
            .WithTeam(team)
            .WithIsActive(true)
            .Build();
    }

    public static OperatorEntity UpdateEntity(this UpdateOperatorRequest request, OperatorEntity entity)
    {
        return entity
            .SetName(request.Name ?? entity.Name)
            .SetEmail(request.Email ?? entity.Email)
            .SetRole(request.Role ?? entity.Role);
    }

    public static GetOperatorResponse ToResponse(this OperatorEntity entity)
    {
        return new GetOperatorResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Role = entity.Role,
            IsActive = entity.IsActive,
            TeamId = entity.TeamId,
            TeamName = entity.Team.Name
        };
    }

    public static PagedResult<GetOperatorResponse> ToResponse(this PagedResult<OperatorEntity> pagedResult)
    {
        return new PagedResult<GetOperatorResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(o => o.ToResponse())
        };
    }
}
