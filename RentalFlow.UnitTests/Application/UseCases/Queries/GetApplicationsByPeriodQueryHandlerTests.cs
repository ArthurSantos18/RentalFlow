namespace RentalFlow.UnitTests.Application.UseCases.Queries;

public sealed class GetApplicationsByPeriodQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IReportRepository> _reportRepositoryMock = new();
    private readonly GetApplicationsByPeriodQueryHandler _handler;

    public GetApplicationsByPeriodQueryHandlerTests()
    {
        _handler = new GetApplicationsByPeriodQueryHandler(
            _reportRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAggregatesExist()
    {
        // Arrange
        var request = _fixture.Create<GetApplicationsByPeriodRequest>();

        var query = _fixture.Build<GetApplicationsByPeriodQuery>()
            .With(q => q.Request, request)
            .Create();

        var aggregates = _fixture
            .CreateMany<ApplicationsByPeriodAggregate>()
            .ToList();

        _reportRepositoryMock
            .Setup(r => r.GetApplicationsByPeriodAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregates);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _reportRepositoryMock.Verify(r => r.GetApplicationsByPeriodAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _reportRepositoryMock.VerifyNoOtherCalls();
    }
}