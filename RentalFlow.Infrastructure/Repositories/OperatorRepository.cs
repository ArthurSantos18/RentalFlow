using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class OperatorRepository(AppDbContext context) : BaseRepository<OperatorEntity>(context), IOperatorRepository
{

}
