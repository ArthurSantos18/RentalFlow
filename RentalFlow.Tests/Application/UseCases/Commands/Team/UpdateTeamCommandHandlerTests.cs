namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class UpdateTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _repositoryMock = new();
    private readonly UpdateTeamCommandHandler _handler;

    public UpdateTeamCommandHandlerTests()
    {
        _handler = new UpdateTeamCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateTeam_WhenExists()
    {
        var teamId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, "Updated Name")
            .With(r => r.Description, "Updated Description")
            .With(r => r.IsActive, false)
            .Create();

        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, teamId)
            .With(c => c.Request, request)
            .Create();

        var existingTeam = TestsFixtures.MakeTeam(id: teamId, name: "Old Name", description: "Old Description");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeam);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingTeam.Name.Should().Be("Updated Name");
        existingTeam.Description.Should().Be("Updated Description");
        existingTeam.IsActive.Should().BeFalse();

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        var teamId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateTeamRequest>();
        var command = _fixture.Build<UpdateTeamCommand>()
            .With(c => c.Id, teamId)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}