namespace RentalFlow.Tests.Application.UseCases.Commands.Team;

public sealed class AddTeamCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
    private readonly AddTeamCommandHandler _handler;

    public AddTeamCommandHandlerTests()
    {
        _handler = new AddTeamCommandHandler(_teamRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamAlreadyExists_WhenTeamNameAlreadyExists()
    {
        var command = _fixture.Create<AddTeamCommand>();

        _teamRepositoryMock
            .Setup(r => r.NameExistsAsync(command.Request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamAlreadyExists);

        _teamRepositoryMock.Verify(r => r.NameExistsAsync(command.Request.Name, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TeamEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddTeam_WhenTeamNameDoesNotExist()
    {
        var command = _fixture.Create<AddTeamCommand>();

        _teamRepositoryMock
            .Setup(r => r.NameExistsAsync(command.Request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _teamRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TeamEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _teamRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _teamRepositoryMock.Verify(r => r.NameExistsAsync(command.Request.Name, It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.AddAsync(It.Is<TeamEntity>(t => t.Name == command.Request.Name), It.IsAny<CancellationToken>()), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _teamRepositoryMock.VerifyNoOtherCalls();
    }
}