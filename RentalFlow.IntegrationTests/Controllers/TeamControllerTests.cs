namespace RentalFlow.IntegrationTests.Controllers;

public sealed class TeamControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetTeamsAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/teams", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTeamsAsync_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/teams", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetTeamResponse>>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task AddTeamAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = Builders.TeamRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/teams", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddTeamAsync_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.TeamRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/teams", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddTeamAsync_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new AddTeamRequest { Name = string.Empty, Description = "Test" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/teams", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddTeamAsync_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var name = $"Team-{Guid.NewGuid():N}"[..20];

        await Client.CreateTeamAsync(name: name);

        var duplicate = Builders.TeamRequest(name: name);

        // Act
        var response = await Client.PostAsJsonAsync("/api/teams", duplicate, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateTeamAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = new UpdateTeamRequest { Name = "Updated Name" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/teams/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateTeamAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();

        var request = new UpdateTeamRequest { Name = "Updated Team", Description = "Updated Description" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/teams/{teamId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateTeamAsync_ShouldReturnNoContent_WhenDeactivatingTeam()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();

        var request = new UpdateTeamRequest { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/teams/{teamId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateTeamAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdateTeamRequest { Name = "Updated Name" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/teams/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTeamAsync_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var existingName = $"Team-{Guid.NewGuid():N}"[..20];
        await Client.CreateTeamAsync(name: existingName);

        var teamId = await Client.CreateTeamAsync();

        var request = new UpdateTeamRequest { Name = existingName };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/teams/{teamId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteTeamAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.DeleteAsync($"/api/teams/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteTeamAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/teams/{teamId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTeamAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/teams/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}