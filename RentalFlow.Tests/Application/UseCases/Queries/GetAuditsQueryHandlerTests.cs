namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetAuditsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IAuditLogRepository> _repositoryMock = new();
    private readonly GetAuditsQueryHandler _handler;

    public GetAuditsQueryHandlerTests()
    {
        _handler = new GetAuditsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAuditsExist()
    {
        var request = _fixture.Create<GetAuditLogRequest>();
        var query = _fixture.Build<GetAuditsQuery>()
            .With(q => q.Request, request)
            .Create();

        var audits = new List<AuditLogEntity>
        {
            _testsFixtures.MakeAuditLog(),
            _testsFixtures.MakeAuditLog()
        };

        var pagedResult = new PagedResult<AuditLogEntity>(
            results: audits,
            totalResults: audits.Count,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetAuditLogsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(60);
        result.Value.TotalResults.Should().Be(audits.Count);
        result.Value.Results.Should().HaveCount(audits.Count);
        result.Value.Results.Should().BeEquivalentTo(
            audits.Select(a => a.ToResponse()));

        _repositoryMock.Verify(
            r => r.GetAuditLogsAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}