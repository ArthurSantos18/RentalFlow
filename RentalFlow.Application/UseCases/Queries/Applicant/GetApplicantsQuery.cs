namespace RentalFlow.Application.UseCases.Queries.Applicant;

public record GetApplicantsQuery(GetApplicantRequest Request) : IQuery<Result<PagedResult<GetApplicantResponse>>>;
