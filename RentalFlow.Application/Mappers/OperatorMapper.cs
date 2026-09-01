using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class OperatorMapper
{
    public static OperatorEntity ToEntity(this AddOperatorRequest request)
    {
        return new OperatorBuilder()
            .WithId(Guid.NewGuid())
            .WithName(request.Name)
            .WithEmail(request.Email)
            .WithRole(request.Role)
            .WithIsActive(true)
            .Build();
    }

    public static OperatorUpdate ToUpdateDomain(this UpdateOperatorRequest request)
    {
        return new OperatorUpdate(
            request.Name,
            request.Email,
            request.Role);
    }

    public static GetOperatorResponse ToResponse(this OperatorEntity entity)
    {
        return new GetOperatorResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Role = entity.Role,
            IsActive = entity.IsActive
        };
    }

    public static PagedResult<GetOperatorResponse> ToResponse(this PagedResult<OperatorEntity> pagedResult)
    {
        return new PagedResult<GetOperatorResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(p => p.ToResponse())
        };
    }
}
