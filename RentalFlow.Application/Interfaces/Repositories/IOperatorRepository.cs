using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Interfaces.Repositories;

public interface IOperatorRepository : IBaseRepository<OperatorEntity>
{
    Task<PagedResult<OperatorEntity>> GetOperatorsAsync(GetOperatorRequest request, CancellationToken cancellationToken);
}
