namespace RentalFlow.Tests.Application.UseCases.Commands.Property;

public sealed class UpdatePropertyCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly Mock<ILogger<UpdatePropertyCommandHandler>> _loggerMock = new();
    private readonly UpdatePropertyCommandHandler _handler;

    public UpdatePropertyCommandHandlerTests()
    {
        _handler = new UpdatePropertyCommandHandler(
            _propertyRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotFound_WhenPropertyDoesNotExist()
    {
        var command = _fixture.Create<UpdatePropertyCommand>();

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _propertyRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProperty_WhenPropertyExists()
    {
        var property = _testsFixtures.MakeProperty();
        var request = _fixture.Create<UpdatePropertyRequest>();

        var command = _fixture.Build<UpdatePropertyCommand>()
            .With(c => c.Id, property.Id)
            .With(c => c.Request, request)
            .Create();

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _propertyRepositoryMock.VerifyNoOtherCalls();
    }
}