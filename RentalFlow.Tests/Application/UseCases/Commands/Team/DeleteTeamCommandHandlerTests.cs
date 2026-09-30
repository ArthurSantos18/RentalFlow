namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class DeleteTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly DeleteTeamCommandHandler _handler;

    public DeleteTeamCommandHandlerTests()
    {
        _handler = new DeleteTeamCommandHandler(
            _teamRepositoryMock.Object,
            _operatorRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamNotFound_WhenTeamDoesNotExist()
    {
        var command = _fixture.Create<DeleteTeamCommand>();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveByTeamAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamHasActiveOperators_WhenTeamHasActiveOperators()
    {
        var team = _testsFixtures.MakeTeam();
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, team.Id)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _operatorRepositoryMock
            .Setup(r => r.CountActiveByTeamAsync(team.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamHasActiveOperators);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveByTeamAsync(team.Id, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        team.IsActive.Should().BeTrue();

        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteTeam_WhenTeamHasNoActiveOperators()
    {
        var team = _testsFixtures.MakeTeam(isActive: false);
        var command = _fixture.Build<DeleteTeamCommand>()
            .With(c => c.Id, team.Id)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _operatorRepositoryMock
            .Setup(r => r.CountActiveByTeamAsync(team.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _teamRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        team.IsActive.Should().BeFalse();

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveByTeamAsync(team.Id, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }
}