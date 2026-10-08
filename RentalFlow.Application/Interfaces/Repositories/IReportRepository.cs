namespace RentalFlow.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<ApplicantAggregate> GetApplicantAggregateAsync(CancellationToken cancellationToken);

    Task<PropertyAggregate> GetPropertyAggregateAsync(CancellationToken cancellationToken);

    Task<RentalApplicationAggregate> GetRentalApplicationAggregateAsync(CancellationToken cancellationToken);

    Task<OperatorAggregate> GetOperatorAggregateAsync(CancellationToken cancellationToken);

    Task<TeamAggregate> GetTeamAggregateAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TopPropertyAggregate>> GetTopPropertyAggregateAsync(GetTopPropertiesRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<TopOperatorAggregate>> GetTopOperatorAggregateAsync(GetTopOperatorsRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<ApplicationsByPeriodAggregate>> GetApplicationsByPeriodAsync(GetApplicationsByPeriodRequest request, CancellationToken cancellationToken);
    Task<ConversionRateAggregate> GetConversionRateAggregateAsync(DateTime from, DateTime to, CancellationToken cancellationToken);
}
