namespace RentalFlow.UnitTests.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordService> _passwordServiceMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<AddOperatorCommandHandler>> _loggerMock = new();

    private readonly AddOperatorCommandHandler _handler;

    public AddOperatorCommandHandlerTests()
    {
        _handler = new AddOperatorCommandHandler(
            _operatorRepositoryMock.Object,
            _teamRepositoryMock.Object,
            _userRepositoryMock.Object,
            _passwordServiceMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidRole_WhenRoleIsNone()
    {
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Role, OperatorRole.None)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRole);

        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnManagerCannotManageAdmin_WhenManagerCreatesAdministrator()
    {
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Role, OperatorRole.Administrator)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.ManagerCannotManageAdmin);

        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamNotFound_WhenTeamDoesNotExist()
    {
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamInactive_WhenTeamIsInactive()
    {
        var team = _testsFixtures.MakeTeam(isActive: false);

        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.TeamId, team.Id)
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamInactive);

        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotInTeam_WhenManagerCreatesOperatorForAnotherTeam()
    {
        var team = _testsFixtures.MakeTeam();
        var managerTeamId = _fixture.Create<Guid>();

        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.TeamId, team.Id)
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(managerTeamId);

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotInTeam);

        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmailAlreadyExists_WhenEmailIsAlreadyInUse()
    {
        var team = _testsFixtures.MakeTeam();

        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.TeamId, team.Id)
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailAlreadyExists);

        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateOperatorAndUser_WhenRequestIsValid()
    {
        var team = _testsFixtures.MakeTeam();

        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.TeamId, team.Id)
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        var command = _fixture.Build<AddOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        var temporaryPassword = _fixture.Create<string>();
        var passwordHash = _fixture.Create<string>();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordServiceMock
            .Setup(s => s.GenerateTemporaryPassword())
            .Returns(temporaryPassword);

        _passwordServiceMock
            .Setup(s => s.Hash(temporaryPassword))
            .Returns(passwordHash);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Email.Should().Be(request.Email.ToLowerInvariant());
        result.Value.Role.Should().Be(request.Role.ToString());
        result.Value.TemporaryPassword.Should().Be(temporaryPassword);

        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _teamRepositoryMock.Verify(r => r.GetByIdAsync(request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.AddAsync(It.Is<OperatorEntity>(o => o.Name == request.Name && o.Role == request.Role && o.TeamId == team.Id), It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.GenerateTemporaryPassword(), Times.Once);
        _passwordServiceMock.Verify(s => s.Hash(temporaryPassword), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<UserEntity>(u => u.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase) && u.PasswordHash == passwordHash), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _currentUserServiceMock.VerifyNoOtherCalls();
        _teamRepositoryMock.VerifyNoOtherCalls();
        _userRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
    }
}