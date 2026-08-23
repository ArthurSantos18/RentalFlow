using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class ApplicantRepository : BaseRepository<ApplicantEntity>, IApplicantRepository
{
    public ApplicantRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ApplicantEntity?> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Cpf == cpf, cancellationToken);
    }
}
