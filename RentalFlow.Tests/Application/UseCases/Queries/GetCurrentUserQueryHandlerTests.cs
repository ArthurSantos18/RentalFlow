namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetCurrentUserQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IUserRepository> _repositoryMock = new();
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _handler = new GetCurrentUserQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var query = new GetCurrentUserQuery(Guid.NewGuid());

        _repositoryMock
            .Setup(x => x.GetUserByIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);

        _repositoryMock.Verify(x => x.GetUserByIdAsync(query.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<UserEntity>();
        var query = new GetCurrentUserQuery(user.Id);

        _repositoryMock
            .Setup(x => x.GetUserByIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(x => x.GetUserByIdAsync(query.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}