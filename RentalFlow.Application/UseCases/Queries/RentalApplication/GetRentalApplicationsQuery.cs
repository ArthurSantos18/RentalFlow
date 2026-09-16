namespace RentalFlow.Application.UseCases.Queries.RentalApplication;

public record GetRentalApplicationsQuery(GetRentalApplicationRequest Request) : IQuery<Result<PagedResult<GetRentalApplicationResponse>>>;
