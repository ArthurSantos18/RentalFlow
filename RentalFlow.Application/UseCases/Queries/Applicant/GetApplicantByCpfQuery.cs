using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Applicant;

public record GetApplicantByCpfQuery(GetApplicantByCpfRequest Request) : IQuery<Result<GetApplicantResponse>>;
