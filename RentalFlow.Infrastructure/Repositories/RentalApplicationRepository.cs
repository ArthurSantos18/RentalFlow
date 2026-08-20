using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class RentalApplicationRepository : BaseRepository<RentalApplicationEntity>, IRentalApplicationRepository
{
    public RentalApplicationRepository(AppDbContext context) : base(context)
    {
    }
}
