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
    public async Task HandleAsync_ShouldSoftDeleteProperty_WhenPropertyExists()
    {
        var propertyId = Guid.NewGuid();
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, propertyId)
            .Create();

        var property = TestsFixtures.MakeProperty(id: propertyId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        property.IsDeleted.Should().BeTrue();
        property.DeletedAt.Should().NotBeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyNotFound()
    {
        var propertyId = Guid.NewGuid();
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, propertyId)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}