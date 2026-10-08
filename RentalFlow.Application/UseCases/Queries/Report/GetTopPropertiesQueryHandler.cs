namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed class GetTopPropertiesQueryHandler(
    IReportRepository _reportRepository,
    IPropertyRepository _propertyRepository
    ) : IQueryHandler<GetTopPropertiesQuery, Result<GetTopPropertiesResponse>>
{
    public async Task<Result<GetTopPropertiesResponse>> HandleAsync(GetTopPropertiesQuery query, CancellationToken cancellationToken)
    {
        var aggregates = await _reportRepository.GetTopPropertyAggregateAsync(query.Request, cancellationToken);

        var propertyIds = aggregates.Select(a => a.PropertyId).ToList();
        var properties = await _propertyRepository.GetByIdsAsync(propertyIds, cancellationToken);

        var propertiesById = properties.ToDictionary(p => p.Id);

        var response = aggregates.ToResponse(propertiesById, query.Request);

        return Result<GetTopPropertiesResponse>.Success(response);
    }
}