using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Interfaces.Repositories;

public interface IRentalApplicationRepository : IBaseRepository<RentalApplicationEntity>
{
    Task<PagedResult<RentalApplicationEntity>> GetRentalApplicationsAsync(GetRentalApplicationRequest request, CancellationToken cancellationToken);
}
