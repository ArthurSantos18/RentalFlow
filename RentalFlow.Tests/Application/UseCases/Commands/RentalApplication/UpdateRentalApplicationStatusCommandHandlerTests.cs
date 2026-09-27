namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly UpdateRentalApplicationStatusCommandHandler _handler;

    public UpdateRentalApplicationStatusCommandHandlerTests()
    {
        _handler = new UpdateRentalApplicationStatusCommandHandler(
            _rentalApplicationRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateStatus_WhenUserIsAdministrator()
    {
        var id = Guid.NewGuid();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = RentalStatus.Approved
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = _testsFixtures.MakeRentalApplication(id: id, status: RentalStatus.Pending);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.Status.Should().Be(RentalStatus.Approved);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateStatus_WhenManagerBelongsToSameTeam()
    {
        var id = Guid.NewGuid();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = RentalStatus.Approved
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);

        var teamId = Guid.NewGuid();
        var @operator = _testsFixtures.MakeOperator(teamId: teamId);
        var existing = _testsFixtures.MakeRentalApplication(id: id, @operator: @operator, status: RentalStatus.Pending);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.Status.Should().Be(RentalStatus.Approved);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDoesNotExist()
    {
        var id = Guid.NewGuid();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenUserRoleIsInvalid()
    {
        var id = Guid.NewGuid();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = _testsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns("InvalidRole");

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenUserIsBroker()
    {
        var id = Guid.NewGuid();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);

        var @operator = _testsFixtures.MakeOperator();
        var existing = _testsFixtures.MakeRentalApplication(id: id, @operator: @operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerBelongsToAnotherTeam()
    {
        var id = Guid.NewGuid();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);

        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var @operator = _testsFixtures.MakeOperator(teamId: otherTeamId);
        var existing = _testsFixtures.MakeRentalApplication(id: id, @operator: @operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.None)]
    [InlineData(RentalStatus.Draft)]
    [InlineData(RentalStatus.Pending)]
    public async Task HandleAsync_ShouldReturnFailure_WhenTransitionFromApprovedIsInvalid(RentalStatus targetStatus)
    {
        var id = Guid.NewGuid();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = targetStatus
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = _testsFixtures.MakeRentalApplication(id: id, status: RentalStatus.Approved);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.None)]
    [InlineData(RentalStatus.Draft)]
    [InlineData(RentalStatus.Pending)]
    public async Task HandleAsync_ShouldReturnFailure_WhenTransitionFromRejectedIsInvalid(RentalStatus targetStatus)
    {
        var id = Guid.NewGuid();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = targetStatus
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = _testsFixtures.MakeRentalApplication(id: id, status: RentalStatus.Rejected);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }
}