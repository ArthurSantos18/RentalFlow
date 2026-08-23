using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Applicant;

public record GetApplicantsQuery() : IQuery<Result<IEnumerable<GetApplicantResponse>>>;
