using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Operator;
using RentalFlow.Domain.Entities.Operator;

namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class AddOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly AddOperatorCommandHandler _handler;

    public AddOperatorCommandHandlerTests()
    {
        _handler = new AddOperatorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddOperator_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = _fixture.Build<AddOperatorCommand>().Create();

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync( It.IsAny<OperatorEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync( It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}