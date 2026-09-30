namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetOperatorByIdQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures;
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<GetOperatorByIdQueryHandler>> _loggerMock = new();
    private readonly GetOperatorByIdQueryHandler _handler;

    public GetOperatorByIdQueryHandlerTests()
    {
        _testsFixtures = new TestsFixtures(_fixture);

        _handler = new GetOperatorByIdQueryHandler(
            _repositoryMock.Object,
            _rentalApplicationRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorIsNotFound()
    {
        var query = _fixture.Create<GetOperatorByIdQuery>();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Never);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserHasInvalidRole()
    {
        var @operator = _testsFixtures.MakeOperator();
        var query = _fixture.Build<GetOperatorByIdQuery>()
            .With(q => q.Id, @operator.Id)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.None));

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenUserIsAdministrator()
    {
        var @operator = _testsFixtures.MakeOperator();

        var applications = _fixture.CreateMany<RentalApplicationEntity>().ToList();

        foreach (var application in applications)
        {
            @operator.AddApplication(application);
        }

        var query = _fixture.Build<GetOperatorByIdQuery>()
            .With(q => q.Id, @operator.Id)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _rentalApplicationRepositoryMock
            .Setup(r => r.CountByOperatorAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator.Applications.Count);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Administrator));

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenManagerAccessesOperatorFromSameTeam()
    {
        var teamId = _fixture.Create<Guid>();
        var @operator = _testsFixtures.MakeOperator(teamId: teamId);

        var applications = _fixture.CreateMany<RentalApplicationEntity>().ToList();

        foreach (var application in applications)
        {
            @operator.AddApplication(application);
        }

        var query = _fixture.Build<GetOperatorByIdQuery>()
            .With(q => q.Id, @operator.Id)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _rentalApplicationRepositoryMock
            .Setup(r => r.CountByOperatorAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator.Applications.Count);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(
            @operator.ToDetailedResponse(@operator.Applications.Count));

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenManagerAccessesOperatorFromAnotherTeam()
    {
        var teamId = _fixture.Create<Guid>();
        var anotherTeamId = _fixture.Create<Guid>();

        var @operator = _testsFixtures.MakeOperator(teamId: anotherTeamId);

        var query = _fixture.Build<GetOperatorByIdQuery>()
            .With(q => q.Id, @operator.Id)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Manager));

        _currentUserServiceMock
            .Setup(s => s.TeamId)
            .Returns(teamId);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenBrokerAccessesOwnOperator()
    {
        var @operator = _testsFixtures.MakeOperator();

        var applications = _fixture.CreateMany<RentalApplicationEntity>().ToList();

        foreach (var application in applications)
        {
            @operator.AddApplication(application);
        }

        var query = _fixture.Build<GetOperatorByIdQuery>()
            .With(q => q.Id, @operator.Id)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _rentalApplicationRepositoryMock
            .Setup(r => r.CountByOperatorAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator.Applications.Count);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(@operator.Id);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(
            @operator.ToDetailedResponse(@operator.Applications.Count));

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenBrokerAccessesAnotherOperator()
    {
        var @operator = _testsFixtures.MakeOperator();
        var currentOperatorId = _fixture.Create<Guid>();

        var query = _fixture.Build<GetOperatorByIdQuery>()
            .With(q => q.Id, @operator.Id)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _currentUserServiceMock
            .Setup(s => s.Role)
            .Returns(nameof(OperatorRole.Broker));

        _currentUserServiceMock
            .Setup(s => s.OperatorId)
            .Returns(currentOperatorId);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Forbidden);

        _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(s => s.Role, Times.Once);
        _currentUserServiceMock.Verify(s => s.OperatorId, Times.Once);
        _currentUserServiceMock.Verify(s => s.TeamId, Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}