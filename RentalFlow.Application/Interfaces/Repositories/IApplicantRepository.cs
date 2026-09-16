namespace RentalFlow.Application.Interfaces.Repositories;

public interface IApplicantRepository : IBaseRepository<ApplicantEntity>
{
    Task<ApplicantEntity?> GetByCpfAsync(string cpf, CancellationToken cancellationToken);
    Task<PagedResult<ApplicantEntity>> GetApplicantsAsync(GetApplicantRequest request, CancellationToken cancellationToken);
}
