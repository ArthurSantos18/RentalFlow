namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetTopOperatorsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IReportRepository> _reportRepositoryMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly GetTopOperatorsQueryHandler _handler;

    public GetTopOperatorsQueryHandlerTests()
    {
        _handler = new GetTopOperatorsQueryHandler(
            _reportRepositoryMock.Object,
            _operatorRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAggregatesExist()
    {
        // Arrange
        var request = _fixture.Create<GetTopOperatorsRequest>();

        var query = _fixture.Build<GetTopOperatorsQuery>()
            .With(q => q.Request, request)
            .Create();

        var operators = new List<OperatorEntity>
        {
            _fixture.Create<OperatorEntity>(),
            _fixture.Create<OperatorEntity>()
        };

        var aggregates = operators
            .Select(operatorEntity => _fixture.Build<TopOperatorAggregate>()
            .With(a => a.OperatorId, operatorEntity.Id)
            .Create())
            .ToList();

        _reportRepositoryMock
            .Setup(r => r.GetTopOperatorAggregateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregates);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdsWithTeamAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(operators);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var operatorIds = aggregates
            .Select(a => a.OperatorId)
            .ToList();

        _reportRepositoryMock.Verify(r => r.GetTopOperatorAggregateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdsWithTeamAsync(operatorIds, It.IsAny<CancellationToken>()), Times.Once);

        _reportRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }
}