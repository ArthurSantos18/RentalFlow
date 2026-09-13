using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Operator;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Errors;
using RentalFlow.Tests.Fixtures;

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
    public async Task HandleAsync_ShouldSoftDeleteOperator_WhenOperatorExists()
    {
        var operatorId = Guid.NewGuid();
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, operatorId)
            .Create();

        var @operator = TestFixtures.MakeOperator(id: operatorId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        @operator.IsDeleted.Should().BeTrue();
        @operator.DeletedAt.Should().NotBeNull();
        @operator.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorNotFound()
    {
        var operatorId = Guid.NewGuid();
        var command = _fixture.Build<DeleteOperatorCommand>()
            .With(c => c.Id, operatorId)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}