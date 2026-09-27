namespace RentalFlow.Tests.Application.UseCases.Commands.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock = new();
    private readonly Mock<IPasswordService> _passwordServiceMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _userTokenRepositoryMock.Object,
            _passwordServiceMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidCredentials_WhenUserDoesNotExist()
    {
        var request = _fixture.Create<LoginRequest>();

        var command = _fixture.Build<LoginCommand>()
            .With(c => c.Request, request)
            .Create();

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<UserEntity>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserInactive_WhenUserIsInactive()
    {
        var user = _testsFixtures.MakeUser(
            isActive: false);

        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, user.Email)
            .Create();

        var command = _fixture.Build<LoginCommand>()
            .With(c => c.Request, request)
            .Create();

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserInactive);

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<UserEntity>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidCredentials_WhenPasswordIsInvalid()
    {
        var user = _testsFixtures.MakeUser(
            isActive: true);

        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, user.Email)
            .Create();

        var command = _fixture.Build<LoginCommand>()
            .With(c => c.Request, request)
            .Create();

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock
            .Setup(p => p.Verify(request.Password, user.PasswordHash))
            .Returns(false);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(p => p.Verify(request.Password, user.PasswordHash), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<UserEntity>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        var user = _testsFixtures.MakeUser(
            isActive: true);

        var request = _fixture.Build<LoginRequest>()
            .With(r => r.Email, user.Email)
            .Create();

        var command = _fixture.Build<LoginCommand>()
            .With(c => c.Request, request)
            .Create();

        var accessToken = _fixture.Create<string>();
        var refreshToken = _fixture.Create<string>();
        var expiresAt = _fixture.Create<DateTime>();

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock
            .Setup(p => p.Verify(request.Password, user.PasswordHash))
            .Returns(true);

        _tokenServiceMock
            .Setup(t => t.GenerateAccessToken(user))
            .Returns(accessToken);

        _tokenServiceMock
            .Setup(t => t.GenerateRefreshToken())
            .Returns(refreshToken);

        _tokenServiceMock
            .Setup(t => t.GetRefreshTokenExpiration())
            .Returns(expiresAt);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be(accessToken);
        result.Value.RefreshToken.Should().Be(refreshToken);
        result.Value.MustChangePassword.Should().Be(user.MustChangePassword);
        result.Value.UserId.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Role.Should().Be(user.Operator?.Role.ToString() ?? "Broker");

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()), Times.Once);
        _passwordServiceMock.Verify(p => p.Verify(request.Password, user.PasswordHash), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateAccessToken(user), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateRefreshToken(), Times.Once);
        _tokenServiceMock.Verify(t => t.GetRefreshTokenExpiration(), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.AddAsync(It.Is<UserTokenEntity>(x => x.User == user && x.RefreshToken == refreshToken && x.ExpiresAt == expiresAt), It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userRepositoryMock.VerifyNoOtherCalls();
        _userTokenRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _tokenServiceMock.VerifyNoOtherCalls();
    }
}