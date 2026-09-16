namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetPropertiesQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPropertyRepository> _repositoryMock = new();
    private readonly GetPropertiesQueryHandler _handler;

    public GetPropertiesQueryHandlerTests()
    {
        _handler = new GetPropertiesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPropertiesExist()
    {
        var request = _fixture.Create<GetPropertyRequest>();
        var query = _fixture.Build<GetPropertiesQuery>()
            .With(q => q.Request, request)
            .Create();

        var properties = new List<PropertyEntity>
        {
            TestsFixtures.MakeProperty(rentPrice: 1500m),
            TestsFixtures.MakeProperty(rentPrice: 3000m, bedrooms: 4)
        };

        var pagedResult = new PagedResult<PropertyEntity>(
            results: properties,
            totalResults: properties.Count,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetPropertiesAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(60);
        result.Value.TotalResults.Should().Be(properties.Count);
        result.Value.Results.Should().HaveCount(properties.Count);
        result.Value.Results.Should().BeEquivalentTo(properties.Select(p => p.ToResponse()));

        _repositoryMock.Verify(r => r.GetPropertiesAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}