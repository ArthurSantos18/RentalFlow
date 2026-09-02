using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.UseCases.Queries.Operator;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Patterns.PagedResult;

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
        // Arrange
        var request = _fixture.Create<GetOperatorsRequest>();
        var query = _fixture.Build<GetOperatorsQuery>()
            .With(q => q.Request, request)
            .Create();

        var @operator = new OperatorBuilder()
            .WithId(Guid.NewGuid())
            .WithName(_fixture.Create<string>())
            .WithEmail(_fixture.Create<string>() + "@test.com")
            .WithRole(OperatorRole.Broker)
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var operators = new List<OperatorEntity> { @operator };
        var pagedResult = new PagedResult<OperatorEntity>(
            results: operators,
            totalResults: 1,
            page: 1,
            pageSize: 60
        );

        _repositoryMock
            .Setup(r => r.GetOperatorsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetOperatorsAsync(request, It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
