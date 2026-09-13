using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Queries.RentalApplication;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Tests.Fixtures;

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
        var request = _fixture.Create<GetRentalApplicationRequest>();
        var query = _fixture.Build<GetRentalApplicationsQuery>()
            .With(q => q.Request, request)
            .Create();

        var applications = new List<RentalApplicationEntity>
        {
            TestFixtures.MakeRentalApplication(financedAmount: 50000m),
            TestFixtures.MakeRentalApplication(financedAmount: 100000m, installments: 24)
        };

        var pagedResult = new PagedResult<RentalApplicationEntity>(
            results: applications,
            totalResults: applications.Count,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetRentalApplicationsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(60);
        result.Value.TotalResults.Should().Be(applications.Count);
        result.Value.Results.Should().HaveCount(applications.Count);

        _repositoryMock.Verify(r => r.GetRentalApplicationsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}