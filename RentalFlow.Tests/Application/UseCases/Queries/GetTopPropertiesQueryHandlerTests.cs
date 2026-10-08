namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetTopPropertiesQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IReportRepository> _reportRepositoryMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly GetTopPropertiesQueryHandler _handler;

    public GetTopPropertiesQueryHandlerTests()
    {
        _handler = new GetTopPropertiesQueryHandler(
            _reportRepositoryMock.Object,
            _propertyRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAggregatesExist()
    {
        // Arrange
        var request = _fixture.Create<GetTopPropertiesRequest>();

        var query = _fixture.Build<GetTopPropertiesQuery>()
            .With(q => q.Request, request)
            .Create();

        var properties = new List<PropertyEntity>
        {
            _fixture.Create<PropertyEntity>(),
            _fixture.Create<PropertyEntity>()
        };

        var aggregates = properties
            .Select(property => _fixture.Build<TopPropertyAggregate>()
            .With(a => a.PropertyId, property.Id)
            .Create())
            .ToList();

        _reportRepositoryMock
            .Setup(r => r.GetTopPropertyAggregateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregates);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var propertyIds = aggregates.Select(a => a.PropertyId).ToList();

        _reportRepositoryMock.Verify(r => r.GetTopPropertyAggregateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdsAsync(propertyIds, It.IsAny<CancellationToken>()), Times.Once);

        _reportRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }
}