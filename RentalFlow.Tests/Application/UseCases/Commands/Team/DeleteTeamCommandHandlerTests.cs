using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Team;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class DeleteTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _repositoryMock = new();
    private readonly DeleteTeamCommandHandler _handler;

    public DeleteTeamCommandHandlerTests()
    {
        _handler = new DeleteTeamCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteTeam_WhenTeamExistsAndIsActive()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, teamId)
            .Create();

        var team = TeamEntity.Empty
            .SetId(teamId)
            .SetIsActive(true);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(team), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenTeamAlreadyInactive()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, teamId)
            .Create();

        var team = TeamEntity.Empty
            .SetId(teamId)
            .SetIsActive(false);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<TeamEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, teamId)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<TeamEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
