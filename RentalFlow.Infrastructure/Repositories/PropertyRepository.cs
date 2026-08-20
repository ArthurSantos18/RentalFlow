using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class PropertyRepository : BaseRepository<PropertyEntity>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context) : base(context)
    {
    }
}
