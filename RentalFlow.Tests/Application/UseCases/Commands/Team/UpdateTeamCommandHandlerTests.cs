namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class UpdateTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly UpdateTeamCommandHandler _handler;

    public UpdateTeamCommandHandlerTests()
    {
        _handler = new UpdateTeamCommandHandler(
            _teamRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamNotFound_WhenTeamDoesNotExist()
    {
        var command = _fixture.Create<UpdateTeamCommand>();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _teamRepositoryMock.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenUserCannotEditTeam()
    {
        var team = _testsFixtures.MakeTeam();
        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, team.Id)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _teamRepositoryMock.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerEditsTeamFromAnotherTeam()
    {
        var team = _testsFixtures.MakeTeam();
        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, team.Id)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(_fixture.Create<Guid>());

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _teamRepositoryMock.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamAlreadyExists_WhenNewNameAlreadyExists()
    {
        var team = _testsFixtures.MakeTeam();
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, _fixture.Create<string>())
            .Create();

        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, team.Id)
            .With(c => c.Request, request)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _teamRepositoryMock
            .Setup(r => r.NameExistsAsync(request.Name!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamAlreadyExists);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _teamRepositoryMock.Verify(r => r.NameExistsAsync(request.Name!, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateTeam_WhenAdministratorEditsTeam()
    {
        var team = _testsFixtures.MakeTeam();
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, _fixture.Create<string>())
            .Create();

        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, team.Id)
            .With(c => c.Request, request)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _teamRepositoryMock
            .Setup(r => r.NameExistsAsync(request.Name!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _teamRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        team.Name.Should().Be(request.Name);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _teamRepositoryMock.Verify(r => r.NameExistsAsync(request.Name!, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateTeam_WhenManagerEditsOwnTeam()
    {
        var team = _testsFixtures.MakeTeam();
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, _fixture.Create<string>())
            .Create();

        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, team.Id)
            .With(c => c.Request, request)
            .Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(team.Id);

        _teamRepositoryMock
            .Setup(r => r.NameExistsAsync(request.Name!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _teamRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        team.Name.Should().Be(request.Name);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _teamRepositoryMock.Verify(r => r.NameExistsAsync(request.Name!, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}