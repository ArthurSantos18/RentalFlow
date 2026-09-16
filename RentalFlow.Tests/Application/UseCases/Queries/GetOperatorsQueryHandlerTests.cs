namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetOperatorsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly GetOperatorsQueryHandler _handler;

    public GetOperatorsQueryHandlerTests()
    {
        _handler = new GetOperatorsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenOperatorsExist()
    {
        var request = _fixture.Create<GetOperatorRequest>();
        var query = _fixture.Build<GetOperatorsQuery>()
            .With(q => q.Request, request)
            .Create();

        var operators = new List<OperatorEntity>
        {
            TestsFixtures.MakeOperator(name: "Operator A"),
            TestsFixtures.MakeOperator(name: "Operator B")
        };

        var pagedResult = new PagedResult<OperatorEntity>(
            results: operators,
            totalResults: operators.Count,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetOperatorsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(60);
        result.Value.TotalResults.Should().Be(operators.Count);
        result.Value.Results.Should().HaveCount(operators.Count);
        result.Value.Results.Should().BeEquivalentTo(operators.Select(o => o.ToResponse()));

        _repositoryMock.Verify(r => r.GetOperatorsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}