using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class OperatorRepository : BaseRepository<OperatorEntity>, IOperatorRepository
{
    public OperatorRepository(AppDbContext context) : base(context)
    {
    }
}
