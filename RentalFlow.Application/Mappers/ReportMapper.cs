namespace RentalFlow.Application.Mappers;

public static class ReportMapper
{
    public static DashboardResponse ToResponse(
        ApplicantAggregate applicants,
        PropertyAggregate properties,
        RentalApplicationAggregate rentalApplications,
        OperatorAggregate operators,
        TeamAggregate teams
        )
    {
        return new DashboardResponse
        {
            Applicants = new ApplicantStats
            {
                Total = applicants.Total,
                Active = applicants.Active,
                Inactive = applicants.Inactive
            },
            Properties = new PropertyStats
            {
                Total = properties.Total,
                Available = properties.Available,
                Rented = properties.Rented,
                Active = properties.Active,
                Inactive = properties.Inactive
            },
            RentalApplications = new RentalApplicationStats
            {
                Total = rentalApplications.Total,
                ByStatus = new RentalApplicationByStatus
                {
                    Draft = rentalApplications.Draft,
                    Pending = rentalApplications.Pending,
                    Approved = rentalApplications.Approved,
                    Rejected = rentalApplications.Rejected
                },
                TotalFinancedAmount = rentalApplications.TotalFinancedAmount,
                TotalAmount = rentalApplications.TotalAmount,
                AverageTicket = rentalApplications.AverageTicket
            },
            Operators = new OperatorStats
            {
                Total = operators.Total,
                Active = operators.Active,
                Inactive = operators.Inactive,
                ByRole = new OperatorByRole
                {
                    Broker = operators.Broker,
                    Manager = operators.Manager,
                    Administrator = operators.Administrator
                }
            },
            Teams = new TeamStats
            {
                Total = teams.Total,
                Active = teams.Active,
                Inactive = teams.Inactive
            },
            ConversionRate = ReportCalculator.ConversionRate(rentalApplications.Approved, rentalApplications.Total)
        };
    }

    public static GetTopPropertiesResponse ToResponse(this IReadOnlyList<TopPropertyAggregate> aggregates, IReadOnlyDictionary<Guid, PropertyEntity> propertiesById, GetTopPropertiesRequest request)
    {
        var items = aggregates
            .Where(a => propertiesById.ContainsKey(a.PropertyId))
            .Select(a => a.ToItem(propertiesById[a.PropertyId]))
            .ToList();

        return new GetTopPropertiesResponse
        {
            From = request.From,
            To = request.To,
            Limit = request.Limit,
            Items = items
        };
    }

    public static TopPropertyItem ToItem(this TopPropertyAggregate aggregate, PropertyEntity property)
    {
        return new TopPropertyItem
        {
            PropertyId = aggregate.PropertyId,
            Address = property.Address.GetFullAddress(),
            RentPrice = property.RentPrice,
            Bedrooms = property.Bedrooms,
            IsAvailable = property.IsAvailable,
            Stats = new TopPropertyStats
            {
                Total = aggregate.Total,
                Approved = aggregate.Approved,
                Pending = aggregate.Pending,
                Rejected = aggregate.Rejected,
                ConversionRate = ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total)
            }
        };
    }

    public static GetTopOperatorsResponse ToResponse(this IReadOnlyList<TopOperatorAggregate> aggregates, IReadOnlyDictionary<Guid, OperatorEntity> operatorsById, GetTopOperatorsRequest request)
    {
        var items = aggregates
            .Where(a => operatorsById.ContainsKey(a.OperatorId))
            .Select(a => a.ToItem(operatorsById[a.OperatorId]))
            .ToList();

        return new GetTopOperatorsResponse
        {
            From = request.From,
            To = request.To,
            Limit = request.Limit,
            Items = items
        };
    }

    public static TopOperatorItem ToItem(this TopOperatorAggregate aggregate, OperatorEntity @operator)
    {
        return new TopOperatorItem
        {
            OperatorId = aggregate.OperatorId,
            Name = @operator.Name,
            Role = @operator.Role.ToString(),
            TeamId = @operator.TeamId,
            TeamName = @operator.Team?.Name ?? string.Empty,
            IsActive = @operator.IsActive,
            Stats = new TopOperatorStats
            {
                Total = aggregate.Total,
                Approved = aggregate.Approved,
                Pending = aggregate.Pending,
                Rejected = aggregate.Rejected,
                TotalAmount = aggregate.TotalAmount,
                AverageTicket = ReportCalculator.AverageTicket(aggregate.TotalAmount, aggregate.Total),
                ConversionRate = ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total)
            }
        };
    }

    public static GetApplicationsByPeriodResponse ToResponse(this IReadOnlyList<ApplicationsByPeriodAggregate> aggregates, GetApplicationsByPeriodRequest request)
    {
        return new GetApplicationsByPeriodResponse
        {
            From = request.From,
            To = request.To,
            GroupBy = request.GroupBy,
            Items = [.. aggregates.Select(a => a.ToItem(request.GroupBy))]
        };
    }

    public static ApplicationPeriodItem ToItem(this ApplicationsByPeriodAggregate aggregate, PeriodGroup groupBy)
    {
        return new ApplicationPeriodItem
        {
            PeriodStart = aggregate.PeriodStart,
            PeriodKey = PeriodBuild.BuildKey(aggregate.PeriodStart, groupBy),
            PeriodLabel = PeriodBuild.BuildLabel(aggregate.PeriodStart, groupBy),
            Applications = new ApplicationPeriodStats
            {
                Total = aggregate.Total,
                Approved = aggregate.Approved,
                Pending = aggregate.Pending,
                Rejected = aggregate.Rejected,
                TotalAmount = aggregate.TotalAmount,
                TotalFinancedAmount = aggregate.TotalFinancedAmount,
                ConversionRate = ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total)
            }
        };
    }

    public static GetConversionRateResponse ToResponse(this ConversionRateAggregate current, DateTime from, DateTime to)
    {
        return new GetConversionRateResponse
        {
            From = from,
            To = to,
            Current = current.ToPeriod(from, to)
        };
    }

    public static GetConversionRateResponse ToResponse(this ConversionRateAggregate current, ConversionRateAggregate previous, DateTime from, DateTime to, DateTime previousFrom, DateTime previousTo)
    {
        return new GetConversionRateResponse
        {
            From = from,
            To = to,
            Current = current.ToPeriod(from, to),
            Previous = previous.ToPeriod(previousFrom, previousTo),
            Comparison = current.ToComparison(previous)
        };
    }

    public static ConversionRatePeriod ToPeriod(this ConversionRateAggregate aggregate, DateTime from, DateTime to)
    {
        return new ConversionRatePeriod
        {
            From = from,
            To = to,
            Total = aggregate.Total,
            Draft = aggregate.Draft,
            Pending = aggregate.Pending,
            Approved = aggregate.Approved,
            Rejected = aggregate.Rejected,
            ConversionRate = ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total)
        };
    }

    public static ConversionRateComparison ToComparison(this ConversionRateAggregate current, ConversionRateAggregate previous)
    {
        var currentRate = ReportCalculator.ConversionRate(current.Approved, current.Total);
        var previousRate = ReportCalculator.ConversionRate(previous.Approved, previous.Total);

        var rateDelta = Math.Round(currentRate - previousRate, 2);

        var changePercentage = previousRate == 0
            ? (currentRate > 0 ? 100 : 0)
            : Math.Round((currentRate - previousRate) / previousRate * 100, 2);

        return new ConversionRateComparison
        {
            TotalDelta = current.Total - previous.Total,
            ApprovedDelta = current.Approved - previous.Approved,
            ConversionRateDelta = rateDelta,
            ConversionRateChangePercentage = changePercentage
        };
    }
}
