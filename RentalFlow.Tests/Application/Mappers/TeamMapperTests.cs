namespace RentalFlow.Tests.Application.Mappers;

public sealed class TeamMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Create<AddTeamRequest>();

        var entity = request.ToEntity();

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be(request.Name);
        entity.Description.Should().Be(request.Description);
        entity.IsActive.Should().BeTrue();
        entity.Operators.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = _testsFixtures.MakeTeam();
        var request = _fixture.Create<UpdateTeamRequest>();

        entity.UpdateFrom(request);

        entity.Name.Should().Be(request.Name);
        entity.Description.Should().Be(request.Description);
        entity.IsActive.Should().Be(request.IsActive!.Value);
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = _testsFixtures.MakeTeam();

        var request = _fixture.Build<UpdateTeamRequest>()
            .Without(r => r.Name)
            .Without(r => r.Description)
            .Without(r => r.IsActive)
            .Create();

        entity.UpdateFrom(request);

        entity.Name.Should().Be(entity.Name);
        entity.Description.Should().Be(entity.Description);
        entity.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = _testsFixtures.MakeTeam();

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Name.Should().Be(entity.Name);
        response.Description.Should().Be(entity.Description);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<TeamEntity>
        {
            _testsFixtures.MakeTeam(),
            _testsFixtures.MakeTeam()
        };

        var pagedResult = new PagedResult<TeamEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(pagedResult.Results.Count());
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}