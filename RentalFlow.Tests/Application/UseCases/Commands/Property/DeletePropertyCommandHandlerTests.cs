using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Property;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Property;

public sealed class DeletePropertyCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPropertyRepository> _repositoryMock = new();
    private readonly DeletePropertyCommandHandler _handler;

    public DeletePropertyCommandHandlerTests()
    {
        _handler = new DeletePropertyCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProperty_WhenPropertyExistsAndIsActive()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, propertyId)
            .Create();

        var property = PropertyEntity.Empty
            .SetId(propertyId)
            .SetIsActive(true);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(property), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPropertyAlreadyInactive()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, propertyId)
            .Create();

        var property = PropertyEntity.Empty
            .SetId(propertyId)
            .SetIsActive(false);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<PropertyEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyNotFound()
    {
        // Arrange
        var expectedError = PropertyErrors.PropertyNotFound;
        var propertyId = Guid.NewGuid();
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, propertyId)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<PropertyEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}