namespace RentalFlow.IntegrationTests.Controllers;

public sealed class OperatorControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetOperatorsAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/operators", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOperatorsAsync_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/operators", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetOperatorResponse>>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOperatorByIdAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync($"/api/operators/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOperatorByIdAsync_ShouldReturnOk_WhenOperatorExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var created = await Client.CreateOperatorAsync(teamId);

        // Act
        var response = await Client.GetAsync($"/api/operators/{created.Id}", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetOperatorByIdResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetOperatorByIdAsync_ShouldReturnNotFound_WhenOperatorDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync($"/api/operators/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = Builders.OperatorRequest(Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var request = Builders.OperatorRequest(teamId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<AddOperatorResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.TemporaryPassword.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var email = TestData.GenerateEmail("duplicated");

        await Client.CreateOperatorAsync(teamId, email: email);

        var request = Builders.OperatorRequest(teamId, email: email);

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.OperatorRequest(Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnBadRequest_WhenRoleIsNone()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var request = Builders.OperatorRequest(teamId, role: OperatorRole.None);

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnBadRequest_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var request = new AddOperatorRequest
        {
            Name = string.Empty,
            Email = string.Empty,
            Role = OperatorRole.Broker,
            TeamId = teamId
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOperatorAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = new UpdateOperatorRequest { Name = "Updated Name" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/operators/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateOperatorAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var created = await Client.CreateOperatorAsync(teamId);

        var request = new UpdateOperatorRequest { Name = "Updated Operator", IsActive = true };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/operators/{created.Id}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateOperatorAsync_ShouldReturnNotFound_WhenOperatorDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdateOperatorRequest { Name = "Updated Name" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/operators/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOperatorAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.DeleteAsync($"/api/operators/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteOperatorAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var created = await Client.CreateOperatorAsync(teamId);

        // Act
        var response = await Client.DeleteAsync($"/api/operators/{created.Id}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOperatorAsync_ShouldReturnNotFound_WhenOperatorDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/operators/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignOperatorAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.PatchAsync($"/api/operators/{Guid.NewGuid()}/team/{Guid.NewGuid()}", null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AssignOperatorAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var newTeamId = await Client.CreateTeamAsync();
        var created = await Client.CreateOperatorAsync(teamId);

        // Act
        var response = await Client.PatchAsync($"/api/operators/{created.Id}/team/{newTeamId}", null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AssignOperatorAsync_ShouldReturnNotFound_WhenOperatorDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();

        // Act
        var response = await Client.PatchAsync($"/api/operators/{Guid.NewGuid()}/team/{teamId}", null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignOperatorAsync_ShouldReturnNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var created = await Client.CreateOperatorAsync(teamId);

        // Act
        var response = await Client.PatchAsync($"/api/operators/{created.Id}/team/{Guid.NewGuid()}", null, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOperatorAsync_ShouldReturnForbidden_WhenManagerTriesToDeleteAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var admin = await Client.CreateOperatorAsync(teamId, role: OperatorRole.Administrator);

        var managerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Manager, teamId);
        SetBearerToken(managerToken);

        // Act
        var response = await Client.DeleteAsync($"/api/operators/{admin.Id}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteOperatorAsync_ShouldReturnForbidden_WhenManagerTriesToDeleteOperatorFromAnotherTeam()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var otherTeamId = await Client.CreateTeamAsync();
        var operatorFromOtherTeam = await Client.CreateOperatorAsync(otherTeamId);

        var myTeamId = await Client.CreateTeamAsync();
        var managerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Manager, myTeamId);
        SetBearerToken(managerToken);

        // Act
        var response = await Client.DeleteAsync($"/api/operators/{operatorFromOtherTeam.Id}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddOperatorAsync_ShouldReturnForbidden_WhenManagerTriesToCreateAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var managerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Manager, teamId);
        SetBearerToken(managerToken);

        var request = Builders.OperatorRequest(teamId, role: OperatorRole.Administrator);

        // Act
        var response = await Client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateOperatorAsync_ShouldReturnForbidden_WhenManagerTriesToPromoteToAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var teamId = await Client.CreateTeamAsync();
        var broker = await Client.CreateOperatorAsync(teamId);

        var managerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Manager, teamId);
        SetBearerToken(managerToken);

        var request = new UpdateOperatorRequest { Role = OperatorRole.Administrator };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/operators/{broker.Id}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}