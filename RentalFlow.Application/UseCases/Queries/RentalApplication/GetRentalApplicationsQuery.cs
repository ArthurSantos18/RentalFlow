namespace RentalFlow.Application.UseCases.Queries.RentalApplication;

public sealed record GetRentalApplicationsQuery(GetRentalApplicationRequest Request) : IQuery<Result<PagedResult<GetRentalApplicationResponse>>>;
