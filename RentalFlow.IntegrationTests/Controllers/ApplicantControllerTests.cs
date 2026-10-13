namespace RentalFlow.IntegrationTests.Controllers;

public sealed class ApplicantControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetApplicantsAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/applicants", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApplicantsAsync_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/applicants", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetApplicantResponse>>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task AddApplicantAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = Builders.ApplicantRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/applicants", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddApplicantAsync_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.ApplicantRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/applicants", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddApplicantAsync_ShouldReturnConflict_WhenCpfAlreadyExists()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var cpf = TestData.GenerateCpf();

        await Client.CreateApplicantAsync(cpf: cpf);

        var duplicate = Builders.ApplicantRequest(cpf: cpf);

        // Act
        var response = await Client.PostAsJsonAsync("/api/applicants", duplicate, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddApplicantAsync_ShouldReturnBadRequest_WhenCpfIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.ApplicantRequest(cpf: "12345678900");

        // Act
        var response = await Client.PostAsJsonAsync("/api/applicants", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddApplicantAsync_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.ApplicantRequest(email: "not-a-valid-email");

        // Act
        var response = await Client.PostAsJsonAsync("/api/applicants", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddApplicantAsync_ShouldReturnBadRequest_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new AddApplicantRequest
        {
            FullName = string.Empty,
            Cpf = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            MonthlyIncome = 0
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/applicants", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateApplicantAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = new UpdateApplicantRequest { FullName = "Updated Name" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/applicants/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateApplicantAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();

        var request = new UpdateApplicantRequest
        {
            FullName = "Updated Name",
            Email = TestData.GenerateEmail("updated"),
            Phone = TestData.GeneratePhone(),
            MonthlyIncome = 7500m
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/applicants/{applicantId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateApplicantAsync_ShouldReturnNoContent_WhenDeactivatingApplicant()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();

        var request = new UpdateApplicantRequest { IsActive = false };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/applicants/{applicantId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateApplicantAsync_ShouldReturnNotFound_WhenApplicantDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdateApplicantRequest { FullName = "Updated Name" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/applicants/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateApplicantAsync_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();

        var request = new UpdateApplicantRequest { Email = "not-a-valid-email" };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/applicants/{applicantId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteApplicantAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.DeleteAsync($"/api/applicants/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteApplicantAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/applicants/{applicantId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteApplicantAsync_ShouldReturnNotFound_WhenApplicantDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/applicants/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}