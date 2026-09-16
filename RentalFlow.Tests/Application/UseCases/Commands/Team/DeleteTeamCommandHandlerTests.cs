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
    public async Task HandleAsync_ShouldSoftDeleteTeam_WhenTeamExists()
    {
        var teamId = Guid.NewGuid();
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, teamId)
            .Create();

        var team = TestsFixtures.MakeTeam(id: teamId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        team.IsDeleted.Should().BeTrue();
        team.DeletedAt.Should().NotBeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        var teamId = Guid.NewGuid();
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, teamId)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}