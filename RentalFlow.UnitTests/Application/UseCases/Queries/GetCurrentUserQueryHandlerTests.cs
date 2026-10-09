namespace RentalFlow.UnitTests.Application.UseCases.Queries;

public sealed class GetCurrentUserQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _repositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<GetCurrentUserQueryHandler>> _loggerMock = new();
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _handler = new GetCurrentUserQueryHandler(
            _repositoryMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCurrentUserQuery();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _repositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);

        _repositoryMock.Verify(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(x => x.UserId, Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<UserEntity>();
        var query = new GetCurrentUserQuery();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(user.Id);

        _repositoryMock
            .Setup(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(x => x.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _currentUserServiceMock.Verify(x => x.UserId, Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}