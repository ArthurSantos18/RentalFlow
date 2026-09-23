namespace RentalFlow.Application.Mappers;

public static class OperatorMapper
{
    public static OperatorEntity ToEntity(this AddOperatorRequest request, TeamEntity team)
    {
        return new OperatorEntity(
            name: request.Name,
            role: request.Role,
            team: team);
    }

    public static OperatorEntity UpdateFrom(this OperatorEntity entity, UpdateOperatorRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            entity.SetName(request.Name);
        }

        if (request.Role.HasValue && request.Role != OperatorRole.None)
        {
            entity.SetRole(request.Role.Value);
        }

        if (request.IsActive.HasValue)
        {
            entity.SetIsActive(request.IsActive.Value);
        }

        return entity.Touch();
    }

    public static AddOperatorResponse ToResponse(this OperatorEntity @operator, UserEntity user, string temporaryPassword)
    {
        return new AddOperatorResponse
        {
            Id = @operator.Id,
            Name = @operator.Name,
            Email = user.Email,
            Role = @operator.Role.ToString(),
            TemporaryPassword = temporaryPassword,
            Message = "Operator created successfully. Share the temporary password with the operator."
        };
    }

    public static GetOperatorResponse ToResponse(this OperatorEntity entity)
    {
        return new GetOperatorResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Role = entity.Role,
            IsActive = entity.IsActive,
            TeamId = entity.TeamId,
            TeamName = entity.Team.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
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
