namespace RentalFlow.Tests.Application.Mappers;

public sealed class OperatorMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Name, "Operator Alpha")
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        var team = TestsFixtures.MakeTeam();

        var entity = request.ToEntity(team);

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be("Operator Alpha");
        entity.Role.Should().Be(OperatorRole.Broker);
        entity.TeamId.Should().Be(team.Id);
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = TestsFixtures.MakeOperator(name: "Old Name", role: OperatorRole.Broker);

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, "New Name")
            .With(r => r.Role, OperatorRole.Manager)
            .With(r => r.IsActive, false)
            .Create();

        entity.UpdateFrom(request);

        entity.Name.Should().Be("New Name");
        entity.Role.Should().Be(OperatorRole.Manager);
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = TestsFixtures.MakeOperator(name: "Existing", role: OperatorRole.Broker);

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, (string?)null)
            .With(r => r.Role, (OperatorRole?)null)
            .With(r => r.IsActive, (bool?)null)
            .Create();

        entity.UpdateFrom(request);

        entity.Name.Should().Be("Existing");
        entity.Role.Should().Be(OperatorRole.Broker);
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var team = TestsFixtures.MakeTeam(name: "Team Alpha");
        var entity = TestsFixtures.MakeOperator(name: "Operator Name", role: OperatorRole.Administrator, team: team);

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Name.Should().Be("Operator Name");
        response.Role.Should().Be(OperatorRole.Administrator);
        response.IsActive.Should().Be(entity.IsActive);
        response.TeamId.Should().Be(team.Id);
        response.TeamName.Should().Be("Team Alpha");
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var team = TestsFixtures.MakeTeam();
        var entities = new List<OperatorEntity>
        {
            TestsFixtures.MakeOperator(team: team),
            TestsFixtures.MakeOperator(name: "Second", team: team)
        };

        var pagedResult = new PagedResult<OperatorEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(2);
        response.PageSize.Should().Be(2);
        response.TotalResults.Should().Be(10);
        response.Results.Should().HaveCount(2);
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}