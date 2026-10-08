namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed record GetTopPropertiesQuery(GetTopPropertiesRequest Request) : IQuery<Result<GetTopPropertiesResponse>>;