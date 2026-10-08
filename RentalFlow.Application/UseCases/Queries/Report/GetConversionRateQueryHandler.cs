namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed class GetConversionRateQueryHandler(IReportRepository _reportRepository) : IQueryHandler<GetConversionRateQuery, Result<GetConversionRateResponse>>
{
    public async Task<Result<GetConversionRateResponse>> HandleAsync(GetConversionRateQuery query, CancellationToken cancellationToken)
    {
        var aggregate = await _reportRepository.GetConversionRateAggregateAsync(query.Request.From, query.Request.To, cancellationToken);

        if (!query.Request.CompareWithPrevious)
        {
            return Result<GetConversionRateResponse>.Success(aggregate.ToResponse(query.Request.From, query.Request.To));
        }

        var (previousFrom, previousTo) = ReportCalculator.CalculatePreviousPeriod(query.Request.From, query.Request.To);

        var previous = await _reportRepository.GetConversionRateAggregateAsync(previousFrom, previousTo, cancellationToken);

        var response = aggregate.ToResponse(previous, query.Request.From, query.Request.To, previousFrom, previousTo);

        return Result<GetConversionRateResponse>.Success(response);
    }
}