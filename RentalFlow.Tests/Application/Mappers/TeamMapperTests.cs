namespace RentalFlow.Tests.Application.Mappers;

public sealed class TeamMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, "Team Alpha")
            .With(r => r.Description, "Description Alpha")
            .Create();

        var entity = request.ToEntity();

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be("Team Alpha");
        entity.Description.Should().Be("Description Alpha");
        entity.IsActive.Should().BeTrue();
        entity.Operators.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = TestsFixtures.MakeTeam(name: "Old Name", description: "Old Description");

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, "Updated Name")
            .With(r => r.Description, "Updated Description")
            .With(r => r.IsActive, false)
            .Create();

        entity.UpdateFrom(request);

        entity.Name.Should().Be("Updated Name");
        entity.Description.Should().Be("Updated Description");
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = TestsFixtures.MakeTeam(name: "Original Name", description: "Original Description");

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, (string?)null)
            .With(r => r.Description, (string?)null)
            .With(r => r.IsActive, (bool?)null)
            .Create();

        entity.UpdateFrom(request);

        entity.Name.Should().Be("Original Name");
        entity.Description.Should().Be("Original Description");
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = TestsFixtures.MakeTeam(name: "Team Name", description: "Team Description");

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Name.Should().Be("Team Name");
        response.Description.Should().Be("Team Description");
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<TeamEntity>
        {
            TestsFixtures.MakeTeam(name: "Team A"),
            TestsFixtures.MakeTeam(name: "Team B")
        };

        var pagedResult = new PagedResult<TeamEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(2);
        response.PageSize.Should().Be(2);
        response.TotalResults.Should().Be(10);
        response.Results.Should().HaveCount(2);
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}