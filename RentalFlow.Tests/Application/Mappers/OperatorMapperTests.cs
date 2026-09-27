namespace RentalFlow.Tests.Application.Mappers;

public sealed class OperatorMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Create<AddOperatorRequest>();
        var team = _testsFixtures.MakeTeam();

        var entity = request.ToEntity(team);

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be(request.Name);
        entity.Role.Should().Be(request.Role);
        entity.TeamId.Should().Be(team.Id);
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = _testsFixtures.MakeOperator();
        var request = _fixture.Create<UpdateOperatorRequest>();

        entity.UpdateFrom(request);

        entity.Name.Should().Be(request.Name);
        entity.Role.Should().Be(request.Role);
        entity.IsActive.Should().Be(request.IsActive!.Value);
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = _testsFixtures.MakeOperator();

        var existingName = entity.Name;
        var existingRole = entity.Role;
        var existingIsActive = entity.IsActive;

        var request = _fixture.Build<UpdateOperatorRequest>()
            .Without(r => r.Name)
            .Without(r => r.Role)
            .Without(r => r.IsActive)
            .Create();

        entity.UpdateFrom(request);

        entity.Name.Should().Be(existingName);
        entity.Role.Should().Be(existingRole);
        entity.IsActive.Should().Be(existingIsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var team = _testsFixtures.MakeTeam();
        var entity = _testsFixtures.MakeOperator(team: team);

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Name.Should().Be(entity.Name);
        response.Role.Should().Be(entity.Role);
        response.IsActive.Should().Be(entity.IsActive);
        response.TeamId.Should().Be(team.Id);
        response.TeamName.Should().Be(team.Name);
        response.CreatedAt.Should().Be(entity.CreatedAt);
        response.Email.Should().Be(entity.User.Email);
        response.MustChangePassword.Should().Be(entity.User.MustChangePassword);
    }

    [Fact]
    public void ToDetailedResponse_ShouldMapGetOperatorByIdResponse()
    {
        var team = _testsFixtures.MakeTeam();
        var entity = _testsFixtures.MakeOperator(team: team);

        entity.AddApplication(_fixture.Create<RentalApplicationEntity>());

        var response = entity.ToDetailedResponse(entity.Applications.Count());

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.IsActive.Should().Be(entity.IsActive);
        response.CreatedAt.Should().Be(entity.CreatedAt);
        response.UpdatedAt.Should().Be(entity.UpdatedAt);
        response.Name.Should().Be(entity.Name);
        response.Role.Should().Be(entity.Role);
        response.TeamId.Should().Be(entity.TeamId);
        response.TeamName.Should().Be(entity.Team.Name);
        response.Email.Should().Be(entity.User.Email);
        response.MustChangePassword.Should().Be(entity.User.MustChangePassword);
        response.UserId.Should().Be(entity.User.Id);
        response.ApplicationsCount.Should().Be(entity.Applications.Count());
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<OperatorEntity>
        {
            _testsFixtures.MakeOperator(),
            _testsFixtures.MakeOperator()
        };

        var pagedResult = new PagedResult<OperatorEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(pagedResult.Results.Count());
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}