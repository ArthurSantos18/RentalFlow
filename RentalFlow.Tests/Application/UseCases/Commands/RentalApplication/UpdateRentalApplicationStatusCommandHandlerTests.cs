namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandlerTests
{
    private readonly Fixture _fixture = new();
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
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = RentalStatus.Approved
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, status: RentalStatus.Pending);

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
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = RentalStatus.Approved
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);

        var teamId = Guid.NewGuid();
        var @operator = TestsFixtures.MakeOperator(teamId: teamId);
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator, status: RentalStatus.Pending);

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
        var id = _fixture.Create<Guid>();
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
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

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
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);

        var @operator = TestsFixtures.MakeOperator();
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator);

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
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationStatusRequest>();
        var command = new UpdateRentalApplicationStatusCommand(id, request);

        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var @operator = TestsFixtures.MakeOperator(teamId: otherTeamId);
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator);

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
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = targetStatus
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, status: RentalStatus.Approved);

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
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest
        {
            RentalStatus = targetStatus
        };

        var command = new UpdateRentalApplicationStatusCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, status: RentalStatus.Rejected);

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