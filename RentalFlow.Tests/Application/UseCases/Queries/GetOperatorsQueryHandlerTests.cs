namespace RentalFlow.Tests.Application.UseCases.Queries.Operator;

public sealed class GetOperatorsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly Mock<IDataScopeService> _dataScopeServiceMock = new();
    private readonly GetOperatorsQueryHandler _handler;

    public GetOperatorsQueryHandlerTests()
    {
        _handler = new GetOperatorsQueryHandler(
            _operatorRepositoryMock.Object,
            _dataScopeServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedOperators()
    {
        var request = _fixture.Create<GetOperatorRequest>();
        var query = _fixture.Build<GetOperatorsQuery>()
            .With(q => q.Request, request)
            .Create();

        var scope = _fixture.Create<DataScope>();
        var operators = new List<OperatorEntity>
        {
            _testsFixtures.MakeOperator(),
            _testsFixtures.MakeOperator()
        };

        var pagedResult = new PagedResult<OperatorEntity>(operators, totalResults: 10, page: 2, pageSize: 2);

        _dataScopeServiceMock
            .Setup(s => s.GetScope())
            .Returns(scope);

        _operatorRepositoryMock
            .Setup(r => r.GetOperatorsAsync(request, scope, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Page.Should().Be(pagedResult.Page);
        result.Value.PageSize.Should().Be(pagedResult.PageSize);
        result.Value.TotalResults.Should().Be(pagedResult.TotalResults);
        result.Value.Results.Should().BeEquivalentTo(operators.Select(o => o.ToResponse()));

        _dataScopeServiceMock.Verify(s => s.GetScope(), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetOperatorsAsync(request, scope, It.IsAny<CancellationToken>()), Times.Once);

        _dataScopeServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }
}