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
            ConversionRate = ConversionRateCalculator.Calculate(rentalApplications.Approved, rentalApplications.Total)
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
            Applications = new TopPropertyStats
            {
                Total = aggregate.Total,
                Approved = aggregate.Approved,
                Pending = aggregate.Pending,
                Rejected = aggregate.Rejected,
                ConversionRate = ConversionRateCalculator.Calculate(aggregate.Approved, aggregate.Total)
            }
        };
    }
}
