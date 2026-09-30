namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class DeleteOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<DeleteOperatorCommandHandler>> _loggerMock = new();
    private readonly DeleteOperatorCommandHandler _handler;

    public DeleteOperatorCommandHandlerTests()
    {
        _handler = new DeleteOperatorCommandHandler(
            _operatorRepositoryMock.Object,
            _userTokenRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotFound_WhenOperatorDoesNotExist()
    {
        var command = _fixture.Create<DeleteOperatorCommand>();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Never);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCannotDeleteSelf_WhenDeletingCurrentOperator()
    {
        var @operator = _testsFixtures.MakeOperator();
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(@operator.Id);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.CannotDeleteSelf);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCannotDeleteLastAdmin_WhenDeletingLastAdministrator()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Administrator);
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(_fixture.Create<Guid>());

        _operatorRepositoryMock
            .Setup(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.CannotDeleteLastAdmin);

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteAdministrator_WhenOtherAdministratorsExist()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Administrator);
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(_fixture.Create<Guid>());

        _operatorRepositoryMock
            .Setup(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _userTokenRepositoryMock
            .Setup(r => r.RevokeAllByUserIdAsync(@operator.User.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _operatorRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.IsActive.Should().BeFalse();
        @operator.User.IsActive.Should().BeFalse();

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(@operator.User.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteOperator_WhenOperatorIsNotAdministrator()
    {
        var @operator = _testsFixtures.MakeOperator(role: OperatorRole.Broker);
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(_fixture.Create<Guid>());

        _userTokenRepositoryMock
            .Setup(r => r.RevokeAllByUserIdAsync(@operator.User.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _operatorRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.IsActive.Should().BeFalse();
        @operator.User.IsActive.Should().BeFalse();

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(@operator.User.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteOperatorWithoutRevokingTokens_WhenOperatorHasNoUser()
    {
        var @operator = _testsFixtures.MakeOperator().SetUser(null!);
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, @operator.Id)
            .Create();

        _operatorRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(_fixture.Create<Guid>());

        _operatorRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.IsActive.Should().BeFalse();

        _operatorRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _operatorRepositoryMock.Verify(r => r.CountActiveAdminsAsync(It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.RevokeAllByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}