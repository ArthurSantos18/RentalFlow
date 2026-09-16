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
    public async Task HandleAsync_ShouldSoftDeleteRentalApplication_WhenExists()
    {
        var id = Guid.NewGuid();
        var command = _fixture.Build<DeleteRentalApplicationCommand>()
            .With(c => c.Id, id)
            .Create();

        var entity = TestsFixtures.MakeRentalApplication(id: id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenNotFound()
    {
        var id = Guid.NewGuid();
        var command = _fixture.Build<DeleteRentalApplicationCommand>()
            .With(c => c.Id, id)
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
}