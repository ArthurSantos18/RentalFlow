using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Applicant;

public sealed class GetApplicantsQueryHandler(IApplicantRepository _applicantRepository) : IQueryHandler<GetApplicantsQuery, Result<IEnumerable<GetApplicantResponse>>>
{
    public async Task<Result<IEnumerable<GetApplicantResponse>>> HandleAsync(GetApplicantsQuery query, CancellationToken cancellationToken)
    {
        var result = await _applicantRepository.GetAllAsync(cancellationToken);

        return Result<IEnumerable<GetApplicantResponse>>.Success(result.ToResponse());
    }
}
