using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class ApplicantRepository : BaseRepository<ApplicantEntity>, IApplicantRepository
{
    public ApplicantRepository(AppDbContext context) : base(context)
    {
    }
}
