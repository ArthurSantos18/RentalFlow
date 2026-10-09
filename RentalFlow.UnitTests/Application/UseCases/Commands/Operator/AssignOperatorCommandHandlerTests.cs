namespace RentalFlow.UnitTests.Application.UseCases.Commands.Operator;

public sealed class AssignOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<AssignOperatorCommandHandler>> _loggerMock = new();
    private readonly AssignOperatorCommandHandler _handler;

    public AssignOperatorCommandHandlerTests()
    {
        _handler = new AssignOperatorCommandHandler(
            _operatorRepositoryMock.Object,
            _teamRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotFound_WhenOperatorDoesNotExist()
    {
        var command = _fixture.Create<AssignOperatorCommand>();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamNotFound_WhenTeamDoesNotExist()
    {
        var @operator = _testsFixtures.MakeOperator();
        var command = _fixture.Create<AssignOperatorCommand>();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamInactive_WhenTeamIsInactive()
    {
        var @operator = _testsFixtures.MakeOperator();
        var team = _testsFixtures.MakeTeam(isActive: false);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, team.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamInactive);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessWithoutSaving_WhenOperatorAlreadyBelongsToTeam()
    {
        var team = _testsFixtures.MakeTeam();
        var @operator = _testsFixtures.MakeOperator(team: team);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, team.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.TeamId.Should().Be(team.Id);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenCurrentUserIsNotAdministratorOrManager()
    {
        var currentTeam = _testsFixtures.MakeTeam(isActive: true);
        var targetTeam = _testsFixtures.MakeTeam(isActive: true);
        var @operator = _testsFixtures.MakeOperator(team: currentTeam);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, targetTeam.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetTeam);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRole);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCannotMoveToDifferentTeam_WhenManagerAssignsToAnotherTeam()
    {
        var currentTeam = _testsFixtures.MakeTeam(isActive: true);
        var targetTeam = _testsFixtures.MakeTeam(isActive: true);
        var @operator = _testsFixtures.MakeOperator(team: currentTeam);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, targetTeam.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetTeam);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(currentTeam.Id);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.CannotMoveToDifferentTeam);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnManagerCannotManageAdmin_WhenManagerAssignsAdministrator()
    {
        var currentTeam = _testsFixtures.MakeTeam(isActive: true);
        var targetTeam = _testsFixtures.MakeTeam(isActive: true);

        var @operator = _testsFixtures.MakeOperator(
            role: OperatorRole.Administrator,
            team: currentTeam);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, targetTeam.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetTeam);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(targetTeam.Id);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.ManagerCannotManageAdmin);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldAssignOperator_WhenAdministratorAssignsToAnotherTeam()
    {
        var currentTeam = _testsFixtures.MakeTeam(isActive: true);
        var targetTeam = _testsFixtures.MakeTeam(isActive: true);
        var @operator = _testsFixtures.MakeOperator(team: currentTeam);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, targetTeam.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetTeam);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.TeamId.Should().Be(targetTeam.Id);
        @operator.Team.Should().BeSameAs(targetTeam);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldAssignOperator_WhenManagerAssignsOperatorToOwnTeam()
    {
        var currentTeam = _testsFixtures.MakeTeam(isActive: true);
        var targetTeam = _testsFixtures.MakeTeam(isActive: true);
        var @operator = _testsFixtures.MakeOperator(team: currentTeam);

        var command = _fixture.Build<AssignOperatorCommand>()
            .With(c => c.TeamId, targetTeam.Id)
            .With(c => c.OperatorId, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetTeam);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(targetTeam.Id);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.TeamId.Should().Be(targetTeam.Id);
        @operator.Team.Should().BeSameAs(targetTeam);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}