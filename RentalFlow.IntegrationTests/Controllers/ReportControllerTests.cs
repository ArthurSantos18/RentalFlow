namespace RentalFlow.IntegrationTests.Controllers;

public sealed class ReportControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetDashboardAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/reports/dashboard", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDashboardAsync_ShouldReturnForbidden_WhenNotAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var brokerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Broker);
        SetBearerToken(brokerToken);

        // Act
        var response = await Client.GetAsync("/api/reports/dashboard", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDashboardAsync_ShouldReturnOk_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/dashboard", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<DashboardResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Applicants.Should().NotBeNull();
        result.Properties.Should().NotBeNull();
        result.RentalApplications.Should().NotBeNull();
        result.Operators.Should().NotBeNull();
        result.Teams.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTopPropertiesAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/reports/top-properties", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopPropertiesAsync_ShouldReturnOk_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/top-properties?limit=10", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetTopPropertiesResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Limit.Should().Be(10);
        result.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTopPropertiesAsync_ShouldReturnBadRequest_WhenLimitIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/top-properties?limit=0", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTopOperatorsAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/reports/top-operators", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopOperatorsAsync_ShouldReturnOk_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/top-operators?limit=10", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetTopOperatorsResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Limit.Should().Be(10);
        result.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTopOperatorsAsync_ShouldReturnBadRequest_WhenLimitIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/top-operators?limit=0", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetApplicationsByPeriodAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/reports/applications-by-period?from=2026-01-01&to=2026-12-31&groupBy=Month", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApplicationsByPeriodAsync_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/applications-by-period?from=2026-01-01&to=2026-12-31&groupBy=Month", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetApplicationsByPeriodResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetApplicationsByPeriodAsync_ShouldReturnBadRequest_WhenToIsBeforeFrom()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/applications-by-period?from=2026-12-31&to=2026-01-01&groupBy=Month", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetApplicationsByPeriodAsync_ShouldReturnBadRequest_WhenGroupByIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/applications-by-period?from=2026-01-01&to=2026-12-31&groupBy=999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConversionRateAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/reports/conversion-rate?from=2026-01-01&to=2026-12-31", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversionRateAsync_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/conversion-rate?from=2026-01-01&to=2026-12-31", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetConversionRateResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Current.Should().NotBeNull();
        result.Previous.Should().BeNull();
    }

    [Fact]
    public async Task GetConversionRateAsync_ShouldReturnOk_WithComparison_WhenRequested()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/conversion-rate?from=2026-01-01&to=2026-12-31&compareWithPrevious=true", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<GetConversionRateResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Current.Should().NotBeNull();
        result.Previous.Should().NotBeNull();
        result.Comparison.Should().NotBeNull();
    }

    [Fact]
    public async Task GetConversionRateAsync_ShouldReturnBadRequest_WhenToIsBeforeFrom()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/conversion-rate?from=2026-12-31&to=2026-01-01", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}