namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class AddTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _repositoryMock = new();
    private readonly AddTeamCommandHandler _handler;

    public AddTeamCommandHandlerTests()
    {
        _handler = new AddTeamCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddTeam_WhenNameIsUnique()
    {
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, "Team Alpha")
            .With(r => r.Description, "Description Alpha")
            .Create();

        var command = _fixture.Build<AddTeamCommand>()
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<TeamEntity>());

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TeamEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, "Team Existing")
            .With(r => r.Description, "Description Existing")
            .Create();

        var command = _fixture.Build<AddTeamCommand>()
            .With(c => c.Request, request)
            .Create();

        var existingTeam = TestsFixtures.MakeTeam(name: request.Name);

        _repositoryMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { existingTeam });

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamDoesExist);

        _repositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<TeamEntity, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TeamEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}