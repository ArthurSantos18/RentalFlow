namespace RentalFlow.Application.Interfaces.Repositories;

public interface IOperatorRepository : IBaseRepository<OperatorEntity>
{
    Task<PagedResult<OperatorEntity>> GetOperatorsAsync(GetOperatorRequest request, CancellationToken cancellationToken);
}
