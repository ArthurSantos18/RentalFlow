using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _repositoryMock = new();
    private readonly UpdateRentalApplicationCommandHandler _handler;

    public UpdateRentalApplicationCommandHandlerTests()
    {
        _handler = new UpdateRentalApplicationCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateRentalApplication_WhenExists()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = _fixture.Build<UpdateRentalApplicationCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .Build();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = _fixture.Build<UpdateRentalApplicationCommand>()
            .With(c => c.Id, id)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
