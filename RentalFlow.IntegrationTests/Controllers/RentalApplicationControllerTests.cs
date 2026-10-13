namespace RentalFlow.IntegrationTests.Controllers;

public sealed class RentalApplicationControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetRentalApplicationsAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/rental-applications", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRentalApplicationsAsync_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/rental-applications", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetRentalApplicationResponse>>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task AddRentalApplicationAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = Builders.RentalApplicationRequest(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddRentalApplicationAsync_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();
        var propertyId = await Client.CreatePropertyAsync();
        var request = Builders.RentalApplicationRequest(applicantId, propertyId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddRentalApplicationAsync_ShouldReturnNotFound_WhenApplicantDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var propertyId = await Client.CreatePropertyAsync();
        var request = Builders.RentalApplicationRequest(Guid.NewGuid(), propertyId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddRentalApplicationAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();
        var request = Builders.RentalApplicationRequest(applicantId, Guid.NewGuid());

        // Act
        var response = await Client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddRentalApplicationAsync_ShouldReturnConflict_WhenPropertyIsNotAvailable()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();
        var propertyId = await Client.CreatePropertyAsync(isAvailable: false);
        var request = Builders.RentalApplicationRequest(applicantId, propertyId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddRentalApplicationAsync_ShouldReturnBadRequest_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new AddRentalApplicationRequest
        {
            ApplicantId = Guid.Empty,
            PropertyId = Guid.Empty,
            FinancedAmount = 0,
            TotalAmount = 0,
            Installments = 0
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRentalApplicationAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = new UpdateRentalApplicationRequest { Installments = 24 };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateRentalApplicationAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicationId = await Client.CreateRentalApplicationAsync();

        var request = new UpdateRentalApplicationRequest { Installments = 24, FinancedAmount = 55000m, TotalAmount = 65000m };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{applicationId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateRentalApplicationAsync_ShouldReturnNotFound_WhenApplicationDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdateRentalApplicationRequest { Installments = 24 };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRentalApplicationAsync_ShouldReturnConflict_WhenApplicationIsApproved()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicationId = await Client.CreateRentalApplicationAsync();

        await Client.ChangeRentalApplicationStatusAsync(applicationId, RentalStatus.Pending);
        await Client.ChangeRentalApplicationStatusAsync(applicationId, RentalStatus.Approved);

        var request = new UpdateRentalApplicationRequest { Installments = 24 };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{applicationId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateRentalApplicationStatusAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{Guid.NewGuid()}/status", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateRentalApplicationStatusAsync_ShouldReturnNoContent_WhenTransitionIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicationId = await Client.CreateRentalApplicationAsync();

        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{applicationId}/status", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateRentalApplicationStatusAsync_ShouldReturnConflict_WhenTransitionIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicationId = await Client.CreateRentalApplicationAsync();

        await Client.ChangeRentalApplicationStatusAsync(applicationId, RentalStatus.Pending);
        await Client.ChangeRentalApplicationStatusAsync(applicationId, RentalStatus.Approved);

        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Draft };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{applicationId}/status", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateRentalApplicationStatusAsync_ShouldReturnNotFound_WhenApplicationDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = RentalStatus.Pending };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/rental-applications/{Guid.NewGuid()}/status", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRentalApplicationAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.DeleteAsync($"/api/rental-applications/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteRentalApplicationAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicationId = await Client.CreateRentalApplicationAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/rental-applications/{applicationId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteRentalApplicationAsync_ShouldReturnNotFound_WhenApplicationDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/rental-applications/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}