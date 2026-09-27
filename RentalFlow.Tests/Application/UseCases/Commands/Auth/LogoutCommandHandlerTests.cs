namespace RentalFlow.Tests.Application.UseCases.Commands.Auth;

public sealed class LogoutCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock = new();

    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _handler = new LogoutCommandHandler(
            _userTokenRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenTokenDoesNotExist()
    {
        var request = _fixture.Create<LogoutRequest>();

        var command = _fixture.Build<LogoutCommand>()
            .With(c => c.Request, request)
            .Create();

        _userTokenRepositoryMock
            .Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserTokenEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _userTokenRepositoryMock.Verify(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _userTokenRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldRevokeTokenAndSaveChanges_WhenTokenExists()
    {
        var user = _testsFixtures.MakeUser(
            isActive: true);

        var request = _fixture.Create<LogoutRequest>();

        var command = _fixture.Build<LogoutCommand>()
            .With(c => c.Request, request)
            .Create();

        var token = _testsFixtures.MakeUserToken(
            user: user,
            refreshToken: request.RefreshToken);

        _userTokenRepositoryMock
            .Setup(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        token.RevokedAt.Should().NotBeNull();

        _userTokenRepositoryMock.Verify(r => r.GetByRefreshTokenAsync(request.RefreshToken, It.IsAny<CancellationToken>()), Times.Once);
        _userTokenRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _userTokenRepositoryMock.VerifyNoOtherCalls();
    }
}