namespace RentalFlow.Application.UseCases.Queries.Applicant;

public sealed record GetApplicantsQuery(GetApplicantRequest Request) : IQuery<Result<PagedResult<GetApplicantResponse>>>;
