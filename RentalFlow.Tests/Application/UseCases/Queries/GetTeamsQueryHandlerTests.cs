using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.UseCases.Queries.Team;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetTeamsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _repositoryMock = new();
    private readonly GetPropertiesQueryHandler _handler;

    public GetTeamsQueryHandlerTests()
    {
        _handler = new GetPropertiesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenTeamsExist()
    {
        // Arrange
        var request = _fixture.Create<GetTeamRequest>();
        var query = _fixture.Build<GetTeamsQuery>()
            .With(q => q.Request, request)
            .Create();

        var team = new TeamBuilder()
            .WithId(Guid.NewGuid())
            .WithName(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var teams = new List<TeamEntity> { team };
        var pagedResult = new PagedResult<TeamEntity>(
            results: teams,
            totalResults: 1,
            page: 1,
            pageSize: 60
        );

        _repositoryMock
            .Setup(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenNoTeamsExist()
    {
        // Arrange
        var request = _fixture.Create<GetTeamRequest>();
        var query = _fixture.Build<GetTeamsQuery>()
            .With(q => q.Request, request)
            .Create();

        var teams = new List<TeamEntity>();
        var pagedResult = new PagedResult<TeamEntity>(
            results: teams,
            totalResults: 0,
            page: 1,
            pageSize: 60
        );

        _repositoryMock
            .Setup(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetTeamsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}
