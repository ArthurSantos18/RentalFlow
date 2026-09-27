namespace RentalFlow.Tests.Application.UseCases.Commands.Auth;

public sealed class ChangePasswordCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordService> _passwordServiceMock = new();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(
            _userRepoMock.Object,
            _passwordServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldChangePassword_WhenCurrentPasswordIsValidAndNewPasswordIsDifferent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = _fixture.Create<string>();
        var newPassword = _fixture.Create<string>();
        var currentPasswordHash = _fixture.Create<string>();
        var newPasswordHash = _fixture.Create<string>();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.CurrentPassword, currentPassword)
            .With(r => r.NewPassword, newPassword)
            .Create();

        var command = _fixture.Build<ChangePasswordCommand>()
            .With(c => c.UserId, userId)
            .With(c => c.Request, request)
            .Create();

        var user = _fixture.Create<UserEntity>();
        user.SetPasswordHash(currentPasswordHash);
        user.SetMustChangePassword(true);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock
            .Setup(s => s.Verify(currentPassword, currentPasswordHash))
            .Returns(true);

        _passwordServiceMock
            .Setup(s => s.Verify(newPassword, currentPasswordHash))
            .Returns(false);

        _passwordServiceMock
            .Setup(s => s.Hash(newPassword))
            .Returns(newPasswordHash);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Password changed successfully.");

        user.PasswordHash.Should().Be(newPasswordHash);
        user.MustChangePassword.Should().BeFalse();

        _userRepoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(currentPassword, currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(newPassword, currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Hash(newPassword), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userRepoMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = _fixture.Build<ChangePasswordCommand>()
            .With(c => c.UserId, userId)
            .Create();

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var result = await _handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);

        _userRepoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _passwordServiceMock.Verify(s => s.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepoMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
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
            .With(c => c.UserId, userId)
            .With(c => c.Request, request)
            .Create();

        var user = _fixture.Create<UserEntity>();
        user.SetPasswordHash(currentPasswordHash);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock
            .Setup(s => s.Verify(currentPassword, currentPasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.HandleAsync(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidPassword);

        _userRepoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(currentPassword, currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(It.IsAny<string>(), currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepoMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenNewPasswordIsSameAsCurrent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = _fixture.Create<string>();
        var newPassword = _fixture.Create<string>();
        var currentPasswordHash = _fixture.Create<string>();

        var request = _fixture.Build<ChangePasswordRequest>()
            .With(r => r.CurrentPassword, currentPassword)
            .With(r => r.NewPassword, newPassword)
            .Create();

        var command = _fixture.Build<ChangePasswordCommand>()
            .With(c => c.UserId, userId)
            .With(c => c.Request, request)
            .Create();

        var user = _fixture.Create<UserEntity>();
        user.SetPasswordHash(currentPasswordHash);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock
            .Setup(s => s.Verify(currentPassword, currentPasswordHash))
            .Returns(true);

        _passwordServiceMock
            .Setup(s => s.Verify(newPassword, currentPasswordHash))
            .Returns(true);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NewPasswordMustBeDifferent);

        _userRepoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(currentPassword, currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Verify(newPassword, currentPasswordHash), Times.Once);
        _passwordServiceMock.Verify(s => s.Hash(It.IsAny<string>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepoMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
    }
}