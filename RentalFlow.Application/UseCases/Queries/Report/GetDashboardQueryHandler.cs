namespace RentalFlow.Application.UseCases.Queries.Report;

public sealed class GetDashboardQueryHandler(IReportRepository _reportRepository) : IQueryHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    public async Task<Result<DashboardResponse>> HandleAsync(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        var applicants = await _reportRepository.GetApplicantAggregateAsync(cancellationToken);
        var properties = await _reportRepository.GetPropertyAggregateAsync(cancellationToken);
        var rentalApplications = await _reportRepository.GetRentalApplicationAggregateAsync(cancellationToken);
        var operators = await _reportRepository.GetOperatorAggregateAsync(cancellationToken);
        var teams = await _reportRepository.GetTeamAggregateAsync(cancellationToken);

        var response = ReportMapper.ToResponse(applicants, properties, rentalApplications, operators, teams);

        return Result<DashboardResponse>.Success(response);
    }
}