using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Interfaces.Repositories;

public interface IApplicantRepository : IBaseRepository<ApplicantEntity>
{
    Task<ApplicantEntity?> GetByCpfAsync(string cpf, CancellationToken cancellationToken);
    Task<PagedResult<ApplicantEntity>> GetApplicantsAsync(GetApplicantRequest request, CancellationToken cancellationToken);
}
