namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed class GetApplicationsByPeriodQueryHandler(IReportRepository _reportRepository) : IQueryHandler<GetApplicationsByPeriodQuery, Result<GetApplicationsByPeriodResponse>>
{
    public async Task<Result<GetApplicationsByPeriodResponse>> HandleAsync(GetApplicationsByPeriodQuery query, CancellationToken cancellationToken)
    {
        var aggregates = await _reportRepository.GetApplicationsByPeriodAsync(query.Request, cancellationToken);

        var response = aggregates.ToResponse(query.Request);

        return Result<GetApplicationsByPeriodResponse>.Success(response);
    }
}