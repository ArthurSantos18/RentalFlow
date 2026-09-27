namespace RentalFlow.Application.Interfaces.Repositories;

public interface IRentalApplicationRepository : IBaseRepository<RentalApplicationEntity>
{
    Task<PagedResult<RentalApplicationEntity>> GetRentalApplicationsAsync(GetRentalApplicationRequest request, DataScope scope, CancellationToken cancellationToken);
    Task<int> CountByOperatorAsync(Guid id, CancellationToken cancellationToken);
}
