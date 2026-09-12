using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Operator;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
    private readonly AddOperatorCommandHandler _handler;

    public AddOperatorCommandHandlerTests()
    {
        _handler = new AddOperatorCommandHandler(_repositoryMock.Object, _teamRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddOperator_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = _fixture.Build<AddOperatorCommand>().Create();
        var team = TeamEntity.Empty;

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTeamNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        var command = _fixture.Build<AddOperatorCommand>().Create();

        _teamRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Request.TeamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.TeamNotFound);

        _teamRepositoryMock.Verify(r => r.GetByIdAsync(command.Request.TeamId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _teamRepositoryMock.VerifyNoOtherCalls();
        _repositoryMock.VerifyNoOtherCalls();
    }
}