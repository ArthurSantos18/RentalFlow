namespace RentalFlow.Application.Interfaces.Repositories;

public interface IPropertyRepository : IBaseRepository<PropertyEntity>
{
    Task<PagedResult<PropertyEntity>> GetPropertiesAsync(GetPropertyRequest request, CancellationToken cancellationToken);
}
