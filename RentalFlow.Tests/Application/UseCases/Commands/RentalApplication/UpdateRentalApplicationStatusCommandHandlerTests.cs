using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly UpdateRentalApplicationStatusCommandHandler _handler;

    public UpdateRentalApplicationStatusCommandHandlerTests()
    {
        _handler = new UpdateRentalApplicationStatusCommandHandler(_rentalApplicationRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateStatus_WhenExistsAndValidTransition()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };
        var command = _fixture.Build<UpdateRentalApplicationStatusCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(existing), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenRentalApplicationDoesNotExist()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };
        var command = _fixture.Build<UpdateRentalApplicationStatusCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenInvalidStatusTransition()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };
        var command = _fixture.Build<UpdateRentalApplicationStatusCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Approved)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.InvalidStatusTransition);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }
}
