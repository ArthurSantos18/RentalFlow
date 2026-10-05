namespace RentalFlow.Tests.Application.UseCases.Commands.Auth;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<ILogger<RefreshTokenCommandHandler>> _loggerMock = new();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(
            _userTokenRepositoryMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidRefreshToken_WhenTokenDoesNotExist()
    {
        var request = _fixture.Create<RefreshTokenRequest>();

        var command = _fixture.Build<RefreshTokenCommand>()
            .With(c => c.Request, request)
            .Create();

        _userTokenRepositoryMock
            .Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTokenEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRefreshToken);

        _userTokenRepositoryMock.Verify(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<UserEntity>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Never);

        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidRefreshToken_WhenTokenIsInvalid()
    {
        var user = _testsFixtures.MakeUser(
            isActive: true);

        var request = _fixture.Create<RefreshTokenRequest>();

        var command = _fixture.Build<RefreshTokenCommand>()
            .With(c => c.Request, request)
            .Create();

        var token = _testsFixtures.MakeUserToken(
            user: user,
            refreshToken: request.RefreshToken,
            expiresAt: DateTime.UtcNow.AddDays(-1));

        _userTokenRepositoryMock
            .Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRefreshToken);

        _userTokenRepositoryMock.Verify(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<UserEntity>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Never);

        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserInactive_WhenUserIsInactive()
    {
        var user = _testsFixtures.MakeUser(
            isActive: false);

        var request = _fixture.Create<RefreshTokenRequest>();

        var command = _fixture.Build<RefreshTokenCommand>()
            .With(c => c.Request, request)
            .Create();

        var token = _testsFixtures.MakeUserToken(
            user: user,
            refreshToken: request.RefreshToken,
            expiresAt: DateTime.UtcNow.AddDays(7));

        _userTokenRepositoryMock
            .Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);

        _userTokenRepositoryMock.Verify(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<UserEntity>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Never);

        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnLoginResponse_WhenRefreshTokenIsValid()
    {
        var user = _testsFixtures.MakeUser(
            isActive: true);

        var request = _fixture.Create<RefreshTokenRequest>();

        var command = _fixture.Build<RefreshTokenCommand>()
            .With(c => c.Request, request)
            .Create();

        var token = _testsFixtures.MakeUserToken(
            user: user,
            refreshToken: request.RefreshToken,
            expiresAt: DateTime.UtcNow.AddDays(7));

        var newAccessToken = _fixture.Create<string>();
        var newRefreshToken = _fixture.Create<string>();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        _userTokenRepositoryMock
            .Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        _tokenServiceMock
            .Setup(t => t.GenerateAccessToken(user))
            .Returns(newAccessToken);

        _tokenServiceMock
            .Setup(t => t.GenerateRefreshToken())
            .Returns(newRefreshToken);

        _tokenServiceMock
            .Setup(t => t.GetRefreshTokenExpiration())
            .Returns(expiresAt);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be(newAccessToken);
        result.Value.RefreshToken.Should().Be(newRefreshToken);
        result.Value.MustChangePassword.Should().Be(user.MustChangePassword);
        result.Value.UserId.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Role.Should().Be(user.Operator?.Role.ToString() ?? "Broker");

        token.RevokedAt.Should().NotBeNull();

        _userTokenRepositoryMock.Verify(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(user), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Once);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.Is<UserTokenEntity>(x => x.User == user && x.RefreshToken == newRefreshToken && x.ExpiresAt == expiresAt), It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }
}