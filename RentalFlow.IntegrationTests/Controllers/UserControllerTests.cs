namespace RentalFlow.IntegrationTests.Controllers;

public sealed class UserControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnOk_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetCurrentUserResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Email.Should().Be(AdminEmail);
        result.OperatorRole.Should().Be(nameof(OperatorRole.Administrator));
        result.OperatorId.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnOk_WhenAuthenticatedAsManager()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var managerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Manager);
        SetBearerToken(managerToken);

        // Act
        var response = await Client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetCurrentUserResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.OperatorRole.Should().Be(nameof(OperatorRole.Manager));
        result.OperatorId.Should().NotBeNull();
        result.TeamId.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnOk_WhenAuthenticatedAsBroker()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var brokerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Broker);
        SetBearerToken(brokerToken);

        // Act
        var response = await Client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetCurrentUserResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.OperatorRole.Should().Be(nameof(OperatorRole.Broker));
        result.OperatorId.Should().NotBeNull();
    }
}