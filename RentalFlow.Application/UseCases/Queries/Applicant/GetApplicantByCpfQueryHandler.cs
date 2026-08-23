using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Applicant;

public sealed class GetApplicantByCpfQueryHandler(IApplicantRepository _applicantRepository) : IQueryHandler<GetApplicantByCpfQuery, Result<GetApplicantResponse>>
{
    public async Task<Result<GetApplicantResponse>> HandleAsync(GetApplicantByCpfQuery query, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByCpfAsync(query.Request.Cpf, cancellationToken);

        if (applicant is null)
        {
            return Result<GetApplicantResponse>.Failure(ApplicantErrors.ApplicantNotFound);
        }

        return Result<GetApplicantResponse>.Success(applicant.ToResponse());
    }
}
