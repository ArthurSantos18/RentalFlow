using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Applicant;

public sealed class GetApplicantsQueryHandler(IApplicantRepository _applicantRepository) : IQueryHandler<GetApplicantsQuery, Result<PagedResult<GetApplicantResponse>>>
{
    public async Task<Result<PagedResult<GetApplicantResponse>>> HandleAsync(GetApplicantsQuery query, CancellationToken cancellationToken)
    {
        var result = await _applicantRepository.GetApplicantsAsync(query.Request, cancellationToken);

        return Result<PagedResult<GetApplicantResponse>>.Success(result.ToResponse());
    }
}
