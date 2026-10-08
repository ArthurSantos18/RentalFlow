namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed class GetTopOperatorsQueryHandler(
    IReportRepository _reportRepository,
    IOperatorRepository _operatorRepository
    ) : IQueryHandler<GetTopOperatorsQuery, Result<GetTopOperatorsResponse>>
{
    public async Task<Result<GetTopOperatorsResponse>> HandleAsync(GetTopOperatorsQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        var aggregates = await _reportRepository.GetTopOperatorAggregateAsync(request, cancellationToken);

        var operatorIds = aggregates.Select(a => a.OperatorId).ToList();
        var operators = await _operatorRepository.GetByIdsWithTeamAsync(operatorIds, cancellationToken);

        var operatorsById = operators.ToDictionary(o => o.Id);

        var response = aggregates.ToResponse(operatorsById, request);

        return Result<GetTopOperatorsResponse>.Success(response);
    }
}