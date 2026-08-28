using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.UseCases.Commands.Property;
using RentalFlow.Domain.Entities.Property;

namespace RentalFlow.Tests.Application.UseCases.Commands.Property;


public sealed class AddPropertyCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPropertyRepository> _repositoryMock = new();
    private readonly AddPropertyCommandHandler _handler;

    public AddPropertyCommandHandlerTests()
    {
        _handler = new AddPropertyCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddProperty_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = _fixture.Build<AddPropertyCommand>().Create();

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PropertyEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}

