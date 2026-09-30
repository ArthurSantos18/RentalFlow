namespace RentalFlow.Tests.Application.UseCases.Commands.Property;

public sealed class DeletePropertyCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<ILogger<DeletePropertyCommandHandler>> _loggerMock = new();
    private readonly DeletePropertyCommandHandler _handler;

    public DeletePropertyCommandHandlerTests()
    {
        _handler = new DeletePropertyCommandHandler(
            _propertyRepositoryMock.Object,
            _rentalApplicationRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotFound_WhenPropertyDoesNotExist()
    {
        var command = _fixture.Create<DeletePropertyCommand>();

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.PropertyHasApplicationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyHasApplications_WhenPropertyHasApplications()
    {
        var property = _testsFixtures.MakeProperty();
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, property.Id)
            .Create();

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _rentalApplicationRepositoryMock
            .Setup(r => r.PropertyHasApplicationsAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyHasApplications);

        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.PropertyHasApplicationsAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProperty_WhenPropertyHasNoApplications()
    {
        var property = _testsFixtures.MakeProperty(isActive: false);
        var command = _fixture.Build<DeletePropertyCommand>()
            .With(c => c.Id, property.Id)
            .Create();

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _rentalApplicationRepositoryMock
            .Setup(r => r.PropertyHasApplicationsAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _propertyRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        property.IsActive.Should().BeFalse();

        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.PropertyHasApplicationsAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }
}