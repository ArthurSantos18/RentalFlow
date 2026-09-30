namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class UpdateOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<UpdateOperatorCommandHandler>> _loggerMock = new();
    private readonly UpdateOperatorCommandHandler _handler;

    public UpdateOperatorCommandHandlerTests()
    {
        _handler = new UpdateOperatorCommandHandler(
            _operatorRepositoryMock.Object,
            _userRepositoryMock.Object,
            _userTokenRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotFound_WhenOperatorDoesNotExist()
    {
        var command = _fixture.Create<UpdateOperatorCommand>();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenCurrentUserIsNotAdministratorOrManager()
    {
        var @operator = _testsFixtures.MakeOperator();
        var command = _fixture.Create<UpdateOperatorCommand>();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerTriesToManageOperatorFromAnotherTeam()
    {
        var @operator = _testsFixtures.MakeOperator();
        var command = _fixture.Create<UpdateOperatorCommand>();
        var teamId = _fixture.Create<Guid>();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnManagerCannotManageAdmin_WhenManagerTriesToManageAdministrator()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Administrator);
        var command = _fixture.Create<UpdateOperatorCommand>();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(@operator.TeamId);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.ManagerCannotManageAdmin);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCannotPromoteToAdmin_WhenManagerTriesToPromoteOperator()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Broker);

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Role, OperatorRole.Administrator)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(@operator.TeamId);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.CannotPromoteToAdmin);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCannotDeactivateLastAdmin_WhenDeactivatingLastAdministrator()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Administrator);

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.IsActive, false)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _operatorRepositoryMock
            .Setup(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.CannotDeactivateLastAdmin);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCannotDemoteLastAdmin_WhenDemotingLastAdministrator()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Administrator);

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Role, OperatorRole.Manager)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _operatorRepositoryMock
            .Setup(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.CannotDemoteLastAdmin);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmailAlreadyExists_WhenNewEmailIsAlreadyInUse()
    {
        var @operator = _testsFixtures.MakeOperator();
        var newEmail = _fixture.Create<MailAddress>().Address;

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Email, newEmail)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(newEmail.ToLowerInvariant(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailAlreadyExists);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(newEmail.ToLowerInvariant(), It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateEmailAndRevokeTokens_WhenEmailChanges()
    {
        var @operator = _testsFixtures.MakeOperator();
        var oldEmail = @operator.User.Email;
        var newEmail = _fixture.Create<MailAddress>().Address;

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Email, newEmail)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _userRepositoryMock
            .Setup(r => r.EmailExistsAsync(newEmail.ToLowerInvariant(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.User.Email.Should().Be(newEmail.ToLowerInvariant());
        @operator.User.Email.Should().NotBe(oldEmail);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _userRepositoryMock.Verify(r => r.EmailExistsAsync(newEmail.ToLowerInvariant(), It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(@operator.User.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeactivateUser_WhenOperatorIsDeactivated()
    {
        var @operator = _testsFixtures.MakeOperator(isActive: true);

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.IsActive, false)
            .Without(r => r.Email)
            .Without(r => r.Role)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.IsActive.Should().BeFalse();
        @operator.User.IsActive.Should().BeFalse();

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOperatorSuccessfully_WhenAdministratorUpdatesOperator()
    {
        var @operator = _testsFixtures.MakeOperator();

        var request = _fixture.Build<UpdateOperatorRequest>()
            .Without(r => r.Email)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.Name.Should().Be(request.Name);
        @operator.Role.Should().Be(request.Role);
        @operator.IsActive.Should().Be(request.IsActive!.Value);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
    }
}