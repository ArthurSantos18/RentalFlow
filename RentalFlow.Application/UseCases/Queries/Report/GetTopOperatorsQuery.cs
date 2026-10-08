namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed record GetTopOperatorsQuery(GetTopOperatorsRequest Request) : IQuery<Result<GetTopOperatorsResponse>>;