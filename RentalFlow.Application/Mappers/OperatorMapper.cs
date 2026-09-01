using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Entities.Operator;

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
}
