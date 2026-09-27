namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<IApplicantRepository> _applicantRepositoryMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
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
    public async Task HandleAsync_ShouldUpdateRentalApplication_WhenExists()
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            Installments = 24,
            FinancedAmount = 50000,
            TotalAmount = 60000,
            ContractDate = DateTime.UtcNow,
            ApplicantId = null,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.Installments.Should().Be(24);
        existing.FinancedAmount.Should().Be(50000);
        existing.TotalAmount.Should().Be(60000);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDoesNotExist()
    {
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = new UpdateRentalApplicationCommand(id, request);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
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
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenUserRoleIsInvalid()
    {
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns("InvalidRole");

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
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
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerBelongsToAnotherTeam()
    {
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = new UpdateRentalApplicationCommand(id, request);

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

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
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
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenBrokerIsNotTheOwner()
    {
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = new UpdateRentalApplicationCommand(id, request);

        var @operator = TestsFixtures.MakeOperator();
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
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
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.Approved)]
    [InlineData(RentalStatus.Rejected)]
    public async Task HandleAsync_ShouldReturnCannotBeEdited_WhenStatusIsInvalid(RentalStatus status)
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, status: status);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationCannotBeEdited);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.Approved)]
    [InlineData(RentalStatus.Rejected)]
    public async Task HandleAsync_ShouldReturnCannotBeEdited_WhenBrokerEditsNonDraftStatus(RentalStatus status)
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var @operator = TestsFixtures.MakeOperator();
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator, status: status);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(@operator.Id);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationCannotBeEdited);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdate_WhenBrokerEditsOwnDraftApplication()
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            Installments = 24,
            FinancedAmount = 50000,
            TotalAmount = 60000,
            ContractDate = DateTime.UtcNow,
            ApplicantId = null,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var @operator = TestsFixtures.MakeOperator();
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(@operator.Id);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.Installments.Should().Be(24);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdate_WhenManagerBelongsToSameTeam()
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            Installments = 24,
            FinancedAmount = 50000,
            TotalAmount = 60000,
            ContractDate = DateTime.UtcNow,
            ApplicantId = null,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var teamId = Guid.NewGuid();
        var @operator = TestsFixtures.MakeOperator(teamId: teamId);
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.Installments.Should().Be(24);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantNotFound_WhenApplicantDoesNotExist()
    {
        var id = _fixture.Create<Guid>();
        var applicantId = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicantId,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantIsInactive_WhenApplicantIsInactive()
    {
        var id = _fixture.Create<Guid>();
        var applicant = TestsFixtures.MakeApplicant(isActive: false);
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicant.Id,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantAlreadyAssigned_WhenApplicantIsAlreadyAssigned()
    {
        var id = _fixture.Create<Guid>();
        var applicant = TestsFixtures.MakeApplicant();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicant.Id,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, applicant: applicant);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.ApplicantAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateApplicant_WhenApplicantIsValid()
    {
        var id = _fixture.Create<Guid>();
        var applicant = TestsFixtures.MakeApplicant();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicant.Id,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.ApplicantId.Should().Be(applicant.Id);
        existing.Applicant.Should().Be(applicant);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotFound_WhenOperatorDoesNotExist()
    {
        var id = _fixture.Create<Guid>();
        var operatorId = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = operatorId,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorIsInactive_WhenOperatorIsInactive()
    {
        var id = _fixture.Create<Guid>();
        var @operator = TestsFixtures.MakeOperator(isActive: false);
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenBrokerTriesToUpdateOperator()
    {
        var id = _fixture.Create<Guid>();
        var operatorId = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = operatorId,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var ownerOperator = TestsFixtures.MakeOperator();
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: ownerOperator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(ownerOperator.Id);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorAlreadyAssigned_WhenOperatorIsAlreadyAssigned()
    {
        var id = _fixture.Create<Guid>();
        var @operator = TestsFixtures.MakeOperator();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, @operator: @operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.OperatorAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnForbidden_WhenManagerAssignsOperatorFromAnotherTeam()
    {
        var id = _fixture.Create<Guid>();
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();

        var @operator = TestsFixtures.MakeOperator(teamId: otherTeamId);
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOperator_WhenOperatorIsValid()
    {
        var id = _fixture.Create<Guid>();
        var @operator = TestsFixtures.MakeOperator();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.OperatorId.Should().Be(@operator.Id);
        existing.Operator.Should().Be(@operator);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotFound_WhenPropertyDoesNotExist()
    {
        var id = _fixture.Create<Guid>();
        var propertyId = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = propertyId
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyIsInactive_WhenPropertyIsInactive()
    {
        var id = _fixture.Create<Guid>();
        var property = TestsFixtures.MakeProperty(isActive: false, isAvailable: true);
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotAvailable_WhenPropertyIsNotAvailable()
    {
        var id = _fixture.Create<Guid>();
        var property = TestsFixtures.MakeProperty(isActive: true, isAvailable: false);
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotAvailable);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyAlreadyAssigned_WhenPropertyIsAlreadyAssigned()
    {
        var id = _fixture.Create<Guid>();
        var property = TestsFixtures.MakeProperty();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id, property: property);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.PropertyAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProperty_WhenPropertyIsValid()
    {
        var id = _fixture.Create<Guid>();
        var property = TestsFixtures.MakeProperty();
        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.PropertyId.Should().Be(property.Id);
        existing.Property.Should().Be(property);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateAllFields_WhenAllDependenciesAreValid()
    {
        var id = _fixture.Create<Guid>();
        var applicant = TestsFixtures.MakeApplicant();
        var @operator = TestsFixtures.MakeOperator();
        var property = TestsFixtures.MakeProperty();

        var request = new UpdateRentalApplicationRequest
        {
            Installments = 36,
            FinancedAmount = 75000,
            TotalAmount = 90000,
            ContractDate = DateTime.UtcNow,
            ApplicantId = applicant.Id,
            OperatorId = @operator.Id,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);
        var existing = TestsFixtures.MakeRentalApplication(id: id);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

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
        existing.Installments.Should().Be(36);
        existing.FinancedAmount.Should().Be(75000);
        existing.TotalAmount.Should().Be(90000);
        existing.ApplicantId.Should().Be(applicant.Id);
        existing.Applicant.Should().Be(applicant);
        existing.OperatorId.Should().Be(@operator.Id);
        existing.Operator.Should().Be(@operator);
        existing.PropertyId.Should().Be(property.Id);
        existing.Property.Should().Be(property);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
    }
}