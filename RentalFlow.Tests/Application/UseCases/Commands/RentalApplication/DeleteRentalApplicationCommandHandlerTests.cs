using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class DeleteRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _repositoryMock = new();
    private readonly DeleteRentalApplicationCommandHandler _handler;

    public DeleteRentalApplicationCommandHandlerTests()
    {
        _handler = new DeleteRentalApplicationCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteRentalApplication_WhenExistsAndIsActive()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = _fixture.Build<DeleteRentalApplicationCommand>().With(c => c.Id, id).Create();

        var entity = RentalApplicationEntity.Empty.SetId(id).SetIsActive(true);

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAlreadyInactive()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = _fixture.Build<DeleteRentalApplicationCommand>().With(c => c.Id, id).Create();

        var entity = RentalApplicationEntity.Empty.SetId(id).SetIsActive(false);

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = _fixture.Build<DeleteRentalApplicationCommand>().With(c => c.Id, id).Create();

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((RentalApplicationEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
