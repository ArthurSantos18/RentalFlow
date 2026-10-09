namespace RentalFlow.UnitTests.Application.UseCases.Commands.Auth;

public sealed class ChangePasswordCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordService> _passwordServiceMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ILogger<ChangePasswordCommandHandler>> _loggerMock = new();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(
            _userRepoMock.Object,
            _passwordServiceMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = _fixture.Create<ChangePasswordCommand>();

        _currentUserServiceMock
            .Setup(c => c.UserId)
            .Returns(userId);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);

        _userRepoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _passwordServiceMock.Verify(s => s.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.UserId, Times.Once);

        _userRepoMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = _fixture.Create<string>();
        var currentPasswordHash = _fixture.Create<string>();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.CurrentPassword, currentPassword)
            .Create();

        var command = _fixture.Build<ChangePasswordCommand>()
            .With(c => c.Request, request)
            .Create();

        var user = _fixture.Create<UserEntity>();
        user.SetPasswordHash(currentPasswordHash);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _currentUserServiceMock
            .Setup(c => c.UserId)
            .Returns(userId);

        _passwordServiceMock
            .Setup(s => s.Verify(currentPassword, currentPasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidPassword);

        _userRepoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(currentPassword, currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(It.IsAny<string>(), currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _currentUserServiceMock.Verify(c => c.UserId, Times.Once);

        _userRepoMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _currentUserServiceMock.VerifyNoOtherCalls();
    }
}