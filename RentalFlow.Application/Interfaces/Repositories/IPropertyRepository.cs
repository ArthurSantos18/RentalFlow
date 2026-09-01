using RentalFlow.Application.Requests.Property;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Interfaces.Repositories;

public interface IPropertyRepository : IBaseRepository<PropertyEntity>
{
    Task<PagedResult<PropertyEntity>> GetPropertiesAsync(GetPropertyRequest request, CancellationToken cancellationToken);
}
