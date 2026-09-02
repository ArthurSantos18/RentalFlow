using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.UseCases.Commands.Operator;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class UpdateOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly UpdateOperatorCommandHandler _handler;

    public UpdateOperatorCommandHandlerTests()
    {
        _handler = new UpdateOperatorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOperator_WhenExists()
    {
        // Arrange
        var operatorId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateOperatorRequest>();
        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Id, operatorId)
            .With(c => c.Request, request)
            .Create();

        var existingOperator = OperatorEntity.Empty
            .SetId(operatorId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOperator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<OperatorEntity>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenOperatorDoesNotExist()
    {
        // Arrange
        var operatorId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateOperatorRequest>();
        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Id, operatorId)
            .With(c => c.Request, request)
            .Create();

        var expectedError = OperatorErrors.OperatorNotFound;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<OperatorEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}