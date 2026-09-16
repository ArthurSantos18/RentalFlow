namespace RentalFlow.Tests.Application.UseCases.Commands.Operator;

public sealed class UpdateOperatorCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IOperatorRepository> _repositoryMock = new();
    private readonly UpdateOperatorCommandHandler _handler;

    public UpdateOperatorCommandHandlerTests()
    {
        _handler = new UpdateOperatorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOperator_WhenExists()
    {
        var operatorId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Name, "Updated Name")
            .With(r => r.Role, OperatorRole.Manager)
            .With(r => r.IsActive, false)
            .Create();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Id, operatorId)
            .With(c => c.Request, request)
            .Create();

        var existingOperator = TestsFixtures.MakeOperator(
            id: operatorId,
            name: "Old Name",
            role: OperatorRole.Broker);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOperator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingOperator.Name.Should().Be("Updated Name");
        existingOperator.Role.Should().Be(OperatorRole.Manager);
        existingOperator.IsActive.Should().BeFalse();

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenOperatorDoesNotExist()
    {
        var operatorId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateOperatorRequest>();

        var command = _fixture.Build<UpdateOperatorCommand>()
            .With(c => c.Id, operatorId)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}