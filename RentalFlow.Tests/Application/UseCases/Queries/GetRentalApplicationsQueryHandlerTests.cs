using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Queries.RentalApplication;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Tests.Application.UseCases.Queries.RentalApplication;

public sealed class GetRentalApplicationsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _repositoryMock = new();
    private readonly GetRentalApplicationsQueryHandler _handler;

    public GetRentalApplicationsQueryHandlerTests()
    {
        _handler = new GetRentalApplicationsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenRentalApplicationsExist()
    {
        // Arrange
        var request = _fixture.Create<GetRentalApplicationRequest>();
        var query = _fixture.Build<GetRentalApplicationsQuery>().With(q => q.Request, request).Create();

        var entity = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(_fixture.Create<decimal>())
            .WithTotalAmount(_fixture.Create<decimal>())
            .WithInstallments(_fixture.Create<int>())
            .Build();

        var list = new List<RentalApplicationEntity> { entity };
        var paged = new PagedResult<RentalApplicationEntity>(list, 1, 1, 60);

        _repositoryMock.Setup(r => r.GetRentalApplicationsAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetRentalApplicationsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}
