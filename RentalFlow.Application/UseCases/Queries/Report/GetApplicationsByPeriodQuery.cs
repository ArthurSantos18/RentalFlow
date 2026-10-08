namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed record GetApplicationsByPeriodQuery(GetApplicationsByPeriodRequest Request) : IQuery<Result<GetApplicationsByPeriodResponse>>;
