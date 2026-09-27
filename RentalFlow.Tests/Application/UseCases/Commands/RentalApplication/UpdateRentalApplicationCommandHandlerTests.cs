namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<IApplicantRepository> _applicantRepositoryMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly UpdateRentalApplicationCommandHandler _handler;

    public UpdateRentalApplicationCommandHandlerTests()
    {
        _handler = new UpdateRentalApplicationCommandHandler(
            _rentalApplicationRepositoryMock.Object,
            _applicantRepositoryMock.Object,
            _operatorRepositoryMock.Object,
            _propertyRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRentalApplicationNotFound_WhenRentalApplicationDoesNotExist()
    {
        var id = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenUserRoleIsInvalid()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);
        var role = _fixture.Create<string>();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(role);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Once);
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerIsFromAnotherTeam()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var teamId = _fixture.Create<Guid>();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Once);
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Once);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenBrokerDoesNotOwnRentalApplication()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var operatorId = _fixture.Create<Guid>();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(operatorId);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Once);
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRentalApplicationCannotBeEdited_WhenAdministratorTriesToEditApprovedApplication()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Approved);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationCannotBeEdited);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRentalApplicationCannotBeEdited_WhenAdministratorTriesToEditRejectedApplication()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Rejected);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationCannotBeEdited);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRentalApplicationCannotBeEdited_WhenBrokerTriesToEditApprovedApplication()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Approved);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(rentalApplication.OperatorId);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationCannotBeEdited);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRentalApplicationCannotBeEdited_WhenBrokerTriesToEditRejectedApplication()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Rejected);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(rentalApplication.OperatorId);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationCannotBeEdited);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantNotFound_WhenApplicantDoesNotExist()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var applicantId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicantId)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantInactive_WhenApplicantIsInactive()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var applicant = _testsFixtures.MakeApplicant(
            isActive: false);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantAlreadyAssigned_WhenApplicantIsAlreadyAssigned()
    {
        var applicant = _testsFixtures.MakeApplicant();

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            applicant: applicant,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.ApplicantAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateApplicant_WhenApplicantIsValid()
    {
        var currentApplicant = _testsFixtures.MakeApplicant(
            isActive: true);

        var applicant = _testsFixtures.MakeApplicant(
            isActive: true);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            applicant: currentApplicant,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .Without(r => r.OperatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        rentalApplication.ApplicantId.Should().Be(applicant.Id);
        rentalApplication.Applicant.Should().Be(applicant);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotFound_WhenOperatorDoesNotExist()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var operatorId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .With(r => r.OperatorId, operatorId)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(3));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorInactive_WhenOperatorIsInactive()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var @operator = _testsFixtures.MakeOperator(
            isActive: false);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .With(r => r.OperatorId, @operator.Id)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(3));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenBrokerTriesToChangeOperator()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(rentalApplication.OperatorId);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .With(r => r.OperatorId, _fixture.Create<Guid>())
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(3));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerTriesToAssignOperatorFromAnotherTeam()
    {
        var teamId = _fixture.Create<Guid>();
        var anotherTeamId = _fixture.Create<Guid>();

        var currentOperator = _testsFixtures.MakeOperator(
            teamId: teamId);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            @operator: currentOperator,
            status: RentalStatus.Draft);

        var @operator = _testsFixtures.MakeOperator(
            teamId: anotherTeamId,
            isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .With(r => r.OperatorId, @operator.Id)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(4));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorAlreadyAssigned_WhenOperatorIsAlreadyAssigned()
    {
        var @operator = _testsFixtures.MakeOperator(
            isActive: true);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            @operator: @operator,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .With(r => r.OperatorId, @operator.Id)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.OperatorAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(4));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOperator_WhenOperatorIsValid()
    {
        var currentOperator = _testsFixtures.MakeOperator(
            isActive: true);

        var @operator = _testsFixtures.MakeOperator(
            isActive: true);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            @operator: currentOperator,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .With(r => r.OperatorId, @operator.Id)
            .Without(r => r.PropertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        rentalApplication.OperatorId.Should().Be(@operator.Id);
        rentalApplication.Operator.Should().Be(@operator);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(4));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotFound_WhenPropertyDoesNotExist()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var propertyId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .With(r => r.PropertyId, propertyId)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyInactive_WhenPropertyIsInactive()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var property = _testsFixtures.MakeProperty(
            isActive: false);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .With(r => r.PropertyId, property.Id)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotAvailable_WhenPropertyIsNotAvailable()
    {
        var rentalApplication = _testsFixtures.MakeRentalApplication(
            status: RentalStatus.Draft);

        var property = _testsFixtures.MakeProperty(
            isAvailable: false);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .With(r => r.PropertyId, property.Id)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotAvailable);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyAlreadyAssigned_WhenPropertyIsAlreadyAssigned()
    {
        var property = _testsFixtures.MakeProperty(
            isActive: true,
            isAvailable: true);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            property: property,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .With(r => r.PropertyId, property.Id)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.PropertyAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProperty_WhenPropertyIsValid()
    {
        var currentProperty = _testsFixtures.MakeProperty(
            isActive: true,
            isAvailable: true);

        var property = _testsFixtures.MakeProperty(
            isActive: true,
            isAvailable: true);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            property: currentProperty,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.ApplicantId)
            .Without(r => r.OperatorId)
            .With(r => r.PropertyId, property.Id)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        rentalApplication.PropertyId.Should().Be(property.Id);
        rentalApplication.Property.Should().Be(property);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(2));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateRentalApplication_WhenAllFieldsAreValid()
    {
        var currentApplicant = _testsFixtures.MakeApplicant(
            isActive: true);

        var applicant = _testsFixtures.MakeApplicant(
            isActive: true);

        var currentOperator = _testsFixtures.MakeOperator(
            isActive: true);

        var @operator = _testsFixtures.MakeOperator(
            isActive: true);

        var currentProperty = _testsFixtures.MakeProperty(
            isActive: true,
            isAvailable: true);

        var property = _testsFixtures.MakeProperty(
            isActive: true,
            isAvailable: true);

        var rentalApplication = _testsFixtures.MakeRentalApplication(
            applicant: currentApplicant,
            @operator: currentOperator,
            property: currentProperty,
            status: RentalStatus.Draft);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .With(r => r.OperatorId, @operator.Id)
            .With(r => r.PropertyId, property.Id)
            .Create();

        var command = new UpdateRentalApplicationCommand(rentalApplication.Id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplication);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        rentalApplication.ApplicantId.Should().Be(applicant.Id);
        rentalApplication.Applicant.Should().Be(applicant);
        rentalApplication.OperatorId.Should().Be(@operator.Id);
        rentalApplication.Operator.Should().Be(@operator);
        rentalApplication.PropertyId.Should().Be(property.Id);
        rentalApplication.Property.Should().Be(property);
        rentalApplication.Installments.Should().Be(request.Installments);
        rentalApplication.FinancedAmount.Should().Be(request.FinancedAmount);
        rentalApplication.TotalAmount.Should().Be(request.TotalAmount);
        rentalApplication.ContractDate.Should().Be(request.ContractDate);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(c => c.Role, Times.Exactly(4));
        _currentUserServiceMock.Verify(c => c.TeamId, Times.Never);
        _currentUserServiceMock.Verify(c => c.OperatorId, Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}