namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class AddRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IRentalApplicationRepository> _rentalRepoMock = new();
    private readonly Mock<IApplicantRepository> _applicantRepoMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepoMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<AddRentalApplicationCommandHandler>> _loggerMock = new();
    private readonly AddRentalApplicationCommandHandler _handler;

    public AddRentalApplicationCommandHandlerTests()
    {
        _handler = new AddRentalApplicationCommandHandler(
            _rentalRepoMock.Object,
            _applicantRepoMock.Object,
            _propertyRepoMock.Object,
            _operatorRepoMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddRentalApplication_WhenAllDependenciesExist()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var property = _testsFixtures.MakeProperty(id: request.PropertyId, isActive: true, isAvailable: true);
        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddRentalApplication_WhenAdministratorOmitsOperatorId()
    {
        var currentOperatorId = Guid.NewGuid();
        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.OperatorId, (Guid?)null)
            .Create();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var property = _testsFixtures.MakeProperty(id: request.PropertyId, isActive: true, isAvailable: true);
        var @operator = _testsFixtures.MakeOperator(id: currentOperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(currentOperatorId);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(currentOperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _operatorRepoMock.Verify(r => r.GetByIdAsync(currentOperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddRentalApplication_WhenManagerRequestsOperatorFromOwnTeam()
    {
        var teamId = Guid.NewGuid();
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var property = _testsFixtures.MakeProperty(id: request.PropertyId, isActive: true, isAvailable: true);
        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true, teamId: teamId);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddRentalApplication_WhenBrokerRequestsForHimself()
    {
        var currentOperatorId = Guid.NewGuid();
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var property = _testsFixtures.MakeProperty(id: request.PropertyId, isActive: true, isAvailable: true);
        var @operator = _testsFixtures.MakeOperator(id: currentOperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(currentOperatorId);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(currentOperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _operatorRepoMock.Verify(r => r.GetByIdAsync(currentOperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenCurrentUserRoleIsInvalid()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns("InvalidRole");

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRole);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorNotFound()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorIsInactive()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: false);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorInactive);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenManagerRequestsOperatorFromAnotherTeam()
    {
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true, teamId: otherTeamId);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(Guid.NewGuid());

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRole);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenApplicantNotFound()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenApplicantIsInactive()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: false);
        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantInactive);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyNotFound()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyIsInactive()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var property = _testsFixtures.MakeProperty(id: request.PropertyId, isActive: false, isAvailable: true);
        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyInactive);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyIsNotAvailable()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = _testsFixtures.MakeApplicant(id: request.ApplicantId, isActive: true);
        var property = _testsFixtures.MakeProperty(id: request.PropertyId, isActive: true, isAvailable: false);
        var @operator = _testsFixtures.MakeOperator(id: request.OperatorId, isActive: true);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(request.OperatorId!.Value);

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(Guid.NewGuid());

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotAvailable);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId!.Value, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }
}