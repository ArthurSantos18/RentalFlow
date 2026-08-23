using RentalFlow.Domain.Entities.Applicant;

namespace RentalFlow.Application.Interfaces.Repositories;

public interface IApplicantRepository : IBaseRepository<ApplicantEntity>
{
    Task<ApplicantEntity?> GetByCpfAsync(string cpf, CancellationToken cancellationToken);
}
