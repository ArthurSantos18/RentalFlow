using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.UseCases.Commands.Property;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Property;

public sealed class UpdatePropertyCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPropertyRepository> _repositoryMock = new();
    private readonly UpdatePropertyCommandHandler _handler;

    public UpdatePropertyCommandHandlerTests()
    {
        _handler = new UpdatePropertyCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProperty_WhenExists()
    {
        // Arrange
        var propertyId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdatePropertyRequest>();
        var command = _fixture.Build<UpdatePropertyCommand>()
            .With(c => c.Id, propertyId)
            .With(c => c.Request, request)
            .Create();

        var existingProperty = PropertyEntity.Empty
            .SetId(propertyId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<PropertyEntity>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var propertyId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdatePropertyRequest>();
        var command = _fixture.Build<UpdatePropertyCommand>()
            .With(c => c.Id, propertyId)
            .With(c => c.Request, request)
            .Create();

        var expectedError = PropertyErrors.PropertyNotFound;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<PropertyEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}