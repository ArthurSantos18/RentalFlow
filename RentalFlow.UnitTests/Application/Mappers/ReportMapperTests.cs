namespace RentalFlow.UnitTests.Application.Mappers;

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

        response.ConversionRate.Should().Be(ReportCalculator.ConversionRate(rentalApplications.Approved, rentalApplications.Total));
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

        item.Stats.Total.Should().Be(aggregate.Total);
        item.Stats.Approved.Should().Be(aggregate.Approved);
        item.Stats.Pending.Should().Be(aggregate.Pending);
        item.Stats.Rejected.Should().Be(aggregate.Rejected);
        item.Stats.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
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

        response.Stats.Total.Should().Be(aggregate.Total);
        response.Stats.Approved.Should().Be(aggregate.Approved);
        response.Stats.Pending.Should().Be(aggregate.Pending);
        response.Stats.Rejected.Should().Be(aggregate.Rejected);
        response.Stats.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
    }

    [Fact]
    public void ToResponse_ShouldMapTopOperatorsCorrectly()
    {
        // Arrange
        var @operator = _fixture.Create<OperatorEntity>();
        var aggregate = _fixture.Create<TopOperatorAggregate>();
        var request = _fixture.Create<GetTopOperatorsRequest>();

        var aggregates = new List<TopOperatorAggregate>
        {
            aggregate
        };

        var operatorsById = new Dictionary<Guid, OperatorEntity>
        {
            [aggregate.OperatorId] = @operator
        };

        // Act
        var response = ReportMapper.ToResponse(aggregates, operatorsById, request);

        // Assert
        response.Should().NotBeNull();
        response.From.Should().Be(request.From);
        response.To.Should().Be(request.To);
        response.Limit.Should().Be(request.Limit);

        response.Items.Should().ContainSingle();

        var item = response.Items.Single();

        item.OperatorId.Should().Be(aggregate.OperatorId);
        item.Name.Should().Be(@operator.Name);
        item.Role.Should().Be(@operator.Role.ToString());
        item.TeamId.Should().Be(@operator.TeamId);
        item.TeamName.Should().Be(@operator.Team?.Name ?? string.Empty);
        item.IsActive.Should().Be(@operator.IsActive);

        item.Stats.Total.Should().Be(aggregate.Total);
        item.Stats.Approved.Should().Be(aggregate.Approved);
        item.Stats.Pending.Should().Be(aggregate.Pending);
        item.Stats.Rejected.Should().Be(aggregate.Rejected);
        item.Stats.TotalAmount.Should().Be(aggregate.TotalAmount);
        item.Stats.AverageTicket.Should().Be(ReportCalculator.AverageTicket(aggregate.TotalAmount, aggregate.Total));
        item.Stats.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
    }

    [Fact]
    public void ToResponse_ShouldIgnoreTopOperatorsWithoutMatchingOperator()
    {
        // Arrange
        var aggregate = _fixture.Create<TopOperatorAggregate>();
        var operatorsById = new Dictionary<Guid, OperatorEntity>();
        var request = _fixture.Create<GetTopOperatorsRequest>();

        var aggregates = new List<TopOperatorAggregate>
        {
            aggregate
        };

        // Act
        var response = ReportMapper.ToResponse(aggregates, operatorsById, request);

        // Assert
        response.Items.Should().BeEmpty();
    }

    [Fact]
    public void ToItem_ShouldMapTopOperatorCorrectly()
    {
        // Arrange
        var aggregate = _fixture.Create<TopOperatorAggregate>();
        var @operator = _fixture.Create<OperatorEntity>();

        // Act
        var response = aggregate.ToItem(@operator);

        // Assert
        response.Should().NotBeNull();

        response.OperatorId.Should().Be(aggregate.OperatorId);
        response.Name.Should().Be(@operator.Name);
        response.Role.Should().Be(@operator.Role.ToString());
        response.TeamId.Should().Be(@operator.TeamId);
        response.TeamName.Should().Be(@operator.Team?.Name ?? string.Empty);
        response.IsActive.Should().Be(@operator.IsActive);

        response.Stats.Total.Should().Be(aggregate.Total);
        response.Stats.Approved.Should().Be(aggregate.Approved);
        response.Stats.Pending.Should().Be(aggregate.Pending);
        response.Stats.Rejected.Should().Be(aggregate.Rejected);
        response.Stats.TotalAmount.Should().Be(aggregate.TotalAmount);
        response.Stats.AverageTicket.Should().Be(ReportCalculator.AverageTicket(aggregate.TotalAmount, aggregate.Total));
        response.Stats.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
    }

    [Fact]
    public void ToResponse_ShouldMapApplicationsByPeriodCorrectly()
    {
        // Arrange
        var request = _fixture.Build<GetApplicationsByPeriodRequest>()
            .With(r => r.From, DateTime.UtcNow.Date)
            .With(r => r.To, DateTime.UtcNow.Date.AddDays(7))
            .With(r => r.GroupBy, PeriodGroup.Day)
            .Create();

        var aggregates = _fixture.CreateMany<ApplicationsByPeriodAggregate>(3).ToList();

        // Act
        var response = ReportMapper.ToResponse(aggregates, request);

        // Assert
        response.Should().NotBeNull();
        response.From.Should().Be(request.From);
        response.To.Should().Be(request.To);
        response.GroupBy.Should().Be(request.GroupBy);
        response.Items.Should().HaveCount(aggregates.Count);

        for (var i = 0; i < aggregates.Count; i++)
        {
            var aggregate = aggregates[i];
            var item = response.Items[i];

            item.PeriodStart.Should().Be(aggregate.PeriodStart);
            item.PeriodKey.Should().Be(PeriodBuild.BuildKey(aggregate.PeriodStart, request.GroupBy));
            item.PeriodLabel.Should().Be(PeriodBuild.BuildLabel(aggregate.PeriodStart, request.GroupBy));

            item.Applications.Total.Should().Be(aggregate.Total);
            item.Applications.Approved.Should().Be(aggregate.Approved);
            item.Applications.Pending.Should().Be(aggregate.Pending);
            item.Applications.Rejected.Should().Be(aggregate.Rejected);
            item.Applications.TotalAmount.Should().Be(aggregate.TotalAmount);
            item.Applications.TotalFinancedAmount.Should().Be(aggregate.TotalFinancedAmount);
            item.Applications.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
        }
    }

    [Fact]
    public void ToResponse_ShouldReturnEmptyItems_WhenNoApplicationsByPeriodExist()
    {
        // Arrange
        var request = _fixture.Create<GetApplicationsByPeriodRequest>();
        var aggregates = new List<ApplicationsByPeriodAggregate>();

        // Act
        var response = ReportMapper.ToResponse(aggregates, request);

        // Assert
        response.Items.Should().BeEmpty();
        response.From.Should().Be(request.From);
        response.To.Should().Be(request.To);
        response.GroupBy.Should().Be(request.GroupBy);
    }

    [Fact]
    public void ToItem_ShouldMapApplicationPeriodCorrectly()
    {
        // Arrange
        var aggregate = _fixture.Create<ApplicationsByPeriodAggregate>();
        var groupBy = PeriodGroup.Month;

        // Act
        var response = aggregate.ToItem(groupBy);

        // Assert
        response.Should().NotBeNull();
        response.PeriodStart.Should().Be(aggregate.PeriodStart);
        response.PeriodKey.Should().Be(PeriodBuild.BuildKey(aggregate.PeriodStart, groupBy));
        response.PeriodLabel.Should().Be(PeriodBuild.BuildLabel(aggregate.PeriodStart, groupBy));

        response.Applications.Total.Should().Be(aggregate.Total);
        response.Applications.Approved.Should().Be(aggregate.Approved);
        response.Applications.Pending.Should().Be(aggregate.Pending);
        response.Applications.Rejected.Should().Be(aggregate.Rejected);
        response.Applications.TotalAmount.Should().Be(aggregate.TotalAmount);
        response.Applications.TotalFinancedAmount.Should().Be(aggregate.TotalFinancedAmount);
        response.Applications.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
    }

    [Fact]
    public void ToResponse_ShouldMapConversionRateCorrectly()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);
        var aggregate = _fixture.Create<ConversionRateAggregate>();

        // Act
        var response = ReportMapper.ToResponse(aggregate, from, to);

        // Assert
        response.Should().NotBeNull();
        response.From.Should().Be(from);
        response.To.Should().Be(to);

        response.Current.From.Should().Be(from);
        response.Current.To.Should().Be(to);
        response.Current.Total.Should().Be(aggregate.Total);
        response.Current.Draft.Should().Be(aggregate.Draft);
        response.Current.Pending.Should().Be(aggregate.Pending);
        response.Current.Approved.Should().Be(aggregate.Approved);
        response.Current.Rejected.Should().Be(aggregate.Rejected);
        response.Current.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));

        response.Previous.Should().BeNull();
        response.Comparison.Should().BeNull();
    }

    [Fact]
    public void ToResponse_ShouldMapConversionRateComparisonCorrectly()
    {
        // Arrange
        var from = new DateTime(2026, 10, 8);
        var to = from.AddDays(7);
        var previousFrom = from.AddDays(-7).AddTicks(1);
        var previousTo = from.AddTicks(-1);

        var current = _fixture.Build<ConversionRateAggregate>()
            .With(a => a.Total, 100)
            .With(a => a.Draft, 10)
            .With(a => a.Pending, 20)
            .With(a => a.Approved, 50)
            .With(a => a.Rejected, 20)
            .Create();

        var previous = _fixture.Build<ConversionRateAggregate>()
            .With(a => a.Total, 80)
            .With(a => a.Draft, 10)
            .With(a => a.Pending, 20)
            .With(a => a.Approved, 32)
            .With(a => a.Rejected, 18)
            .Create();

        // Act
        var response = ReportMapper.ToResponse(
            current, previous, from, to, previousFrom, previousTo);

        // Assert
        response.Should().NotBeNull();
        response.From.Should().Be(from);
        response.To.Should().Be(to);

        response.Current.From.Should().Be(from);
        response.Current.To.Should().Be(to);
        response.Current.Total.Should().Be(current.Total);
        response.Current.Draft.Should().Be(current.Draft);
        response.Current.Pending.Should().Be(current.Pending);
        response.Current.Approved.Should().Be(current.Approved);
        response.Current.Rejected.Should().Be(current.Rejected);
        response.Current.ConversionRate.Should().Be(50);

        response.Previous.Should().NotBeNull();
        response.Previous!.From.Should().Be(previousFrom);
        response.Previous.To.Should().Be(previousTo);
        response.Previous.Total.Should().Be(previous.Total);
        response.Previous.Draft.Should().Be(previous.Draft);
        response.Previous.Pending.Should().Be(previous.Pending);
        response.Previous.Approved.Should().Be(previous.Approved);
        response.Previous.Rejected.Should().Be(previous.Rejected);
        response.Previous.ConversionRate.Should().Be(40);

        response.Comparison.Should().NotBeNull();
        response.Comparison!.TotalDelta.Should().Be(20);
        response.Comparison.ApprovedDelta.Should().Be(18);
        response.Comparison.ConversionRateDelta.Should().Be(10);
        response.Comparison.ConversionRateChangePercentage.Should().Be(25);
    }

    [Fact]
    public void ToPeriod_ShouldMapConversionRatePeriodCorrectly()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);
        var aggregate = _fixture.Create<ConversionRateAggregate>();

        // Act
        var response = aggregate.ToPeriod(from, to);

        // Assert
        response.Should().NotBeNull();
        response.From.Should().Be(from);
        response.To.Should().Be(to);
        response.Total.Should().Be(aggregate.Total);
        response.Draft.Should().Be(aggregate.Draft);
        response.Pending.Should().Be(aggregate.Pending);
        response.Approved.Should().Be(aggregate.Approved);
        response.Rejected.Should().Be(aggregate.Rejected);
        response.ConversionRate.Should().Be(ReportCalculator.ConversionRate(aggregate.Approved, aggregate.Total));
    }

    [Fact]
    public void ToComparison_ShouldCalculateDeltasCorrectly()
    {
        // Arrange
        var current = _fixture.Build<ConversionRateAggregate>()
            .With(a => a.Total, 100)
            .With(a => a.Approved, 60)
            .Create();

        var previous = _fixture.Build<ConversionRateAggregate>()
            .With(a => a.Total, 80)
            .With(a => a.Approved, 32)
            .Create();

        // Act
        var response = current.ToComparison(previous);

        // Assert
        response.TotalDelta.Should().Be(20);
        response.ApprovedDelta.Should().Be(28);
        response.ConversionRateDelta.Should().Be(20);
        response.ConversionRateChangePercentage.Should().Be(50);
    }

    [Theory]
    [InlineData(10, 0, 100)]
    [InlineData(0, 0, 0)]
    public void ToComparison_ShouldHandleZeroPreviousConversionRate(
        int currentApproved,
        int previousApproved,
        double expectedChangePercentage)
    {
        // Arrange
        var current = _fixture.Build<ConversionRateAggregate>()
            .With(a => a.Total, 100)
            .With(a => a.Approved, currentApproved)
            .Create();

        var previous = _fixture.Build<ConversionRateAggregate>()
            .With(a => a.Total, 0)
            .With(a => a.Approved, previousApproved)
            .Create();

        // Act
        var response = current.ToComparison(previous);

        // Assert
        response.ConversionRateChangePercentage.Should().Be(expectedChangePercentage);
    }
}
