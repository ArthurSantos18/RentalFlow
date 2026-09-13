using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Application.UseCases.Commands.Team;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class UpdateTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _repositoryMock = new();
    private readonly UpdateTeamCommandHandler _handler;

    public UpdateTeamCommandHandlerTests()
    {
        _handler = new UpdateTeamCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateTeam_WhenExists()
    {
        // Arrange
        var teamId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateTeamRequest>();
        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, teamId)
            .With(c => c.Request, request)
            .Create();

        var existingTeam = TeamEntity.Empty
            .SetId(teamId)
            .SetName("Old Name")
            .SetDescription("Old Description")
            .SetIsActive(true);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeam);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<TeamEntity>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        var teamId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateTeamRequest>();
        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, teamId)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock.Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<TeamEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
