namespace RentalFlow.Tests.Application.Mappers;

public sealed class ReportMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToResponse_ShouldMapCorrectly()
    {
        // Arrange
        var applicants = _fixture.Create<ApplicantAggregate>();
        var properties = _fixture.Create<PropertyAggregate>();
        var rentalApplications = _fixture.Create<RentalApplicationAggregate>();
        var operators = _fixture.Create<OperatorAggregate>();
        var teams = _fixture.Create<TeamAggregate>();

        // Act
        var response = ReportMapper.ToResponse(applicants, properties, rentalApplications, operators, teams);

        // Assert
        response.Should().NotBeNull();

        response.Applicants.Total.Should().Be(applicants.Total);
        response.Applicants.Active.Should().Be(applicants.Active);
        response.Applicants.Inactive.Should().Be(applicants.Inactive);

        response.Properties.Total.Should().Be(properties.Total);
        response.Properties.Available.Should().Be(properties.Available);
        response.Properties.Rented.Should().Be(properties.Rented);
        response.Properties.Active.Should().Be(properties.Active);
        response.Properties.Inactive.Should().Be(properties.Inactive);

        response.RentalApplications.Total.Should().Be(rentalApplications.Total);
        response.RentalApplications.ByStatus.Draft.Should().Be(rentalApplications.Draft);
        response.RentalApplications.ByStatus.Pending.Should().Be(rentalApplications.Pending);
        response.RentalApplications.ByStatus.Approved.Should().Be(rentalApplications.Approved);
        response.RentalApplications.ByStatus.Rejected.Should().Be(rentalApplications.Rejected);
        response.RentalApplications.TotalFinancedAmount.Should().Be(rentalApplications.TotalFinancedAmount);
        response.RentalApplications.TotalAmount.Should().Be(rentalApplications.TotalAmount);
        response.RentalApplications.AverageTicket.Should().Be(rentalApplications.AverageTicket);

        response.Operators.Total.Should().Be(operators.Total);
        response.Operators.Active.Should().Be(operators.Active);
        response.Operators.Inactive.Should().Be(operators.Inactive);
        response.Operators.ByRole.Broker.Should().Be(operators.Broker);
        response.Operators.ByRole.Manager.Should().Be(operators.Manager);
        response.Operators.ByRole.Administrator.Should().Be(operators.Administrator);

        response.Teams.Total.Should().Be(teams.Total);
        response.Teams.Active.Should().Be(teams.Active);
        response.Teams.Inactive.Should().Be(teams.Inactive);

        response.ConversionRate.Should().Be(ConversionRateCalculator.Calculate(rentalApplications.Approved, rentalApplications.Total));
    }

    [Fact]
    public void ToResponse_ShouldMapTopPropertiesCorrectly()
    {
        // Arrange
        var property = _fixture.Create<PropertyEntity>();
        var aggregate = _fixture.Create<TopPropertyAggregate>();
        var request = _fixture.Create<GetTopPropertiesRequest>();

        var aggregates = new List<TopPropertyAggregate>
        {
            aggregate
        };

        var propertiesById = new Dictionary<Guid, PropertyEntity>
        {
            [aggregate.PropertyId] = property
        };

        // Act
        var response = ReportMapper.ToResponse(aggregates, propertiesById, request);

        // Assert
        response.Should().NotBeNull();
        response.From.Should().Be(request.From);
        response.To.Should().Be(request.To);
        response.Limit.Should().Be(request.Limit);

        response.Items.Should().ContainSingle();

        var item = response.Items.Single();

        item.PropertyId.Should().Be(aggregate.PropertyId);
        item.Address.Should().Be(property.Address.GetFullAddress());
        item.RentPrice.Should().Be(property.RentPrice);
        item.Bedrooms.Should().Be(property.Bedrooms);
        item.IsAvailable.Should().Be(property.IsAvailable);

        item.Applications.Total.Should().Be(aggregate.Total);
        item.Applications.Approved.Should().Be(aggregate.Approved);
        item.Applications.Pending.Should().Be(aggregate.Pending);
        item.Applications.Rejected.Should().Be(aggregate.Rejected);
        item.Applications.ConversionRate.Should().Be(ConversionRateCalculator.Calculate(aggregate.Approved, aggregate.Total));
    }

    [Fact]
    public void ToResponse_ShouldIgnoreTopPropertiesWithoutMatchingProperty()
    {
        // Arrange
        var aggregate = _fixture.Create<TopPropertyAggregate>();
        var propertiesById = new Dictionary<Guid, PropertyEntity>();
        var request = _fixture.Create<GetTopPropertiesRequest>();

        var aggregates = new List<TopPropertyAggregate>
        {
            aggregate
        };

        // Act
        var response = ReportMapper.ToResponse(aggregates, propertiesById, request);

        // Assert
        response.Items.Should().BeEmpty();
    }

    [Fact]
    public void ToItem_ShouldMapCorrectly()
    {
        // Arrange
        var aggregate = _fixture.Create<TopPropertyAggregate>();
        var property = _fixture.Create<PropertyEntity>();

        // Act
        var response = aggregate.ToItem(property);

        // Assert
        response.Should().NotBeNull();

        response.PropertyId.Should().Be(aggregate.PropertyId);
        response.Address.Should().Be(property.Address.GetFullAddress());
        response.RentPrice.Should().Be(property.RentPrice);
        response.Bedrooms.Should().Be(property.Bedrooms);
        response.IsAvailable.Should().Be(property.IsAvailable);

        response.Applications.Total.Should().Be(aggregate.Total);
        response.Applications.Approved.Should().Be(aggregate.Approved);
        response.Applications.Pending.Should().Be(aggregate.Pending);
        response.Applications.Rejected.Should().Be(aggregate.Rejected);
        response.Applications.ConversionRate.Should().Be(ConversionRateCalculator.Calculate(aggregate.Approved, aggregate.Total));
    }
}
