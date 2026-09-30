namespace RentalFlow.Application.Interfaces.Repositories;

public interface IOperatorRepository : IBaseRepository<OperatorEntity>
{
    Task<PagedResult<OperatorEntity>> GetOperatorsAsync(GetOperatorRequest request, DataScope scope, CancellationToken cancellationToken);
    Task<OperatorEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<int> CountActiveAdminsAsync(CancellationToken cancellationToken);
    Task<int> CountActiveByTeamAsync(Guid id, CancellationToken cancellationToken);
}