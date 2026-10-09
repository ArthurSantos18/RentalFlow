namespace RentalFlow.UnitTests.Application.UseCases.Queries;

public sealed class GetConversionRateQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IReportRepository> _reportRepositoryMock = new();
    private readonly GetConversionRateQueryHandler _handler;

    public GetConversionRateQueryHandlerTests()
    {
        _handler = new GetConversionRateQueryHandler(_reportRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenCompareWithPreviousIsFalse()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        var request = _fixture.Build<GetConversionRateRequest>()
            .With(r => r.From, from)
            .With(r => r.To, to)
            .With(r => r.CompareWithPrevious, false)
            .Create();

        var query = _fixture.Build<GetConversionRateQuery>()
            .With(q => q.Request, request)
            .Create();

        var aggregate = _fixture.Create<ConversionRateAggregate>();

        _reportRepositoryMock
            .Setup(r => r.GetConversionRateAggregateAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _reportRepositoryMock.Verify(r => r.GetConversionRateAggregateAsync(from, to, It.IsAny<CancellationToken>()),
            Times.Once);

        _reportRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenCompareWithPreviousIsTrue()
    {
        // Arrange
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(7);

        var request = _fixture.Build<GetConversionRateRequest>()
            .With(r => r.From, from)
            .With(r => r.To, to)
            .With(r => r.CompareWithPrevious, true)
            .Create();

        var query = _fixture.Build<GetConversionRateQuery>()
            .With(q => q.Request, request)
            .Create();

        var aggregate = _fixture.Create<ConversionRateAggregate>();
        var previousAggregate = _fixture.Create<ConversionRateAggregate>();

        var (previousFrom, previousTo) = ReportCalculator.CalculatePreviousPeriod(from, to);

        _reportRepositoryMock
            .Setup(r => r.GetConversionRateAggregateAsync(from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregate);

        _reportRepositoryMock
            .Setup(r => r.GetConversionRateAggregateAsync(previousFrom, previousTo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousAggregate);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _reportRepositoryMock.Verify(r => r.GetConversionRateAggregateAsync(from, to, It.IsAny<CancellationToken>()), Times.Once);
        _reportRepositoryMock.Verify(r => r.GetConversionRateAggregateAsync(previousFrom, previousTo, It.IsAny<CancellationToken>()), Times.Once);

        _reportRepositoryMock.VerifyNoOtherCalls();
    }
}