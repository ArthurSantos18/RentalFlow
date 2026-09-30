namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class DeleteRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IRentalApplicationRepository> _rentalRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<DeleteRentalApplicationCommandHandler>> _loggerMock = new();
    private readonly DeleteRentalApplicationCommandHandler _handler;

    public DeleteRentalApplicationCommandHandlerTests()
    {
        _handler = new DeleteRentalApplicationCommandHandler(
            _rentalRepoMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteRentalApplication_WhenUserIsAdministrator()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        var rentalApplication = _testsFixtures.MakeRentalApplication(id: command.Id, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteRentalApplication_WhenManagerBelongsToSameTeam()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        var teamId = Guid.NewGuid();
        var @operator = _testsFixtures.MakeOperator(teamId: teamId);
        var rentalApplication = _testsFixtures.MakeRentalApplication(id: command.Id, @operator: @operator, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenRentalApplicationIsAlreadyInactive()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        var rentalApplication = _testsFixtures.MakeRentalApplication(id: command.Id, isActive: false);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenRentalApplicationNotFound()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserRoleIsInvalid()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        var rentalApplication = _testsFixtures.MakeRentalApplication(id: command.Id, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns("InvalidRole");

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserIsBroker()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        var rentalApplication = _testsFixtures.MakeRentalApplication(id: command.Id, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenManagerBelongsToAnotherTeam()
    {
        var command = _fixture.Create<DeleteRentalApplicationCommand>();

        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var @operator = _testsFixtures.MakeOperator(teamId: otherTeamId);
        var rentalApplication = _testsFixtures.MakeRentalApplication(id: command.Id, @operator: @operator, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _rentalRepoMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalRepoMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalRepoMock.VerifyNoOtherCalls();
    }
}