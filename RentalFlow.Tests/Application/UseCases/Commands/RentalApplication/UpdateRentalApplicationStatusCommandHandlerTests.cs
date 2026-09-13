using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Errors;
using RentalFlow.Tests.Fixtures;

namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationStatusCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _repositoryMock = new();
    private readonly UpdateRentalApplicationStatusCommandHandler _handler;

    public UpdateRentalApplicationStatusCommandHandlerTests()
    {
        _handler = new UpdateRentalApplicationStatusCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateStatus_WhenExistsAndValidTransition()
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };
        var command = _fixture.Build<UpdateRentalApplicationStatusCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        var existing = TestFixtures.MakeRentalApplication(id: id, status: RentalStatus.Draft);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.Status.Should().Be(RentalStatus.Pending);

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenRentalApplicationDoesNotExist()
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };
        var command = _fixture.Build<UpdateRentalApplicationStatusCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenInvalidStatusTransition()
    {
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };
        var command = _fixture.Build<UpdateRentalApplicationStatusCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        var existing = TestFixtures.MakeRentalApplication(id: id, status: RentalStatus.Approved);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.InvalidStatusTransition);

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}