namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed record GetConversionRateQuery(GetConversionRateRequest Request) : IQuery<Result<GetConversionRateResponse>>;
