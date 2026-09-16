namespace RentalFlow.Application.Interfaces.Repositories;

public interface IRentalApplicationRepository : IBaseRepository<RentalApplicationEntity>
{
    Task<PagedResult<RentalApplicationEntity>> GetRentalApplicationsAsync(GetRentalApplicationRequest request, CancellationToken cancellationToken);
}
