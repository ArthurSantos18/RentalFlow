namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetTeamsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _repositoryMock = new();
    private readonly GetTeamsQueryHandler _handler;

    public GetTeamsQueryHandlerTests()
    {
        _handler = new GetTeamsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenTeamsExist()
    {
        var request = _fixture.Create<GetTeamRequest>();
        var query = _fixture.Build<GetTeamsQuery>()
            .With(q => q.Request, request)
            .Create();

        var teams = new List<TeamEntity>
        {
            TestsFixtures.MakeTeam(name: "Team A"),
            TestsFixtures.MakeTeam(name: "Team B")
        };

        var pagedResult = new PagedResult<TeamEntity>(
            results: teams,
            totalResults: teams.Count,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(60);
        result.Value.TotalResults.Should().Be(teams.Count);
        result.Value.Results.Should().HaveCount(teams.Count);
        result.Value.Results.Should().BeEquivalentTo(teams.Select(t => t.ToResponse()));

        _repositoryMock.Verify(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenNoTeamsExist()
    {
        var request = _fixture.Create<GetTeamRequest>();
        var query = _fixture.Build<GetTeamsQuery>()
            .With(q => q.Request, request)
            .Create();

        var pagedResult = new PagedResult<TeamEntity>(
            results: [],
            totalResults: 0,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalResults.Should().Be(0);
        result.Value.Results.Should().BeEmpty();

        _repositoryMock.Verify(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}