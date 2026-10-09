namespace RentalFlow.UnitTests.Application.UseCases.Queries;

public sealed class GetAuditLogsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IAuditLogRepository> _repositoryMock = new();
    private readonly GetAuditLogsQueryHandler _handler;

    public GetAuditLogsQueryHandlerTests()
    {
        _handler = new GetAuditLogsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAuditsExist()
    {
        // Arrange
        var request = _fixture.Create<GetAuditLogRequest>();
        var query = _fixture.Build<GetAuditLogsQuery>()
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

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetAuditLogsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}