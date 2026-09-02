using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Operator;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class DeleteOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly DeleteOperatorCommandHandler _handler;

    public DeleteOperatorCommandHandlerTests()
    {
        _handler = new DeleteOperatorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteOperator_WhenOperatorExistsAndIsActive()
    {
        // Arrange
        var operatorId = Guid.NewGuid();
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, operatorId)
            .Create();

        var @operator = OperatorEntity.Empty
            .SetId(operatorId)
            .SetIsActive(true);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(@operator), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenOperatorAlreadyInactive()
    {
        // Arrange
        var operatorId = Guid.NewGuid();
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, operatorId)
            .Create();

        var @operator = OperatorEntity.Empty
            .SetId(operatorId)
            .SetIsActive(false);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<OperatorEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorNotFound()
    {
        // Arrange
        var expectedError = OperatorErrors.OperatorNotFound;
        var operatorId = Guid.NewGuid();

        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, operatorId)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<OperatorEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}