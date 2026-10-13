namespace RentalFlow.IntegrationTests.Controllers;

public sealed class AuditLogControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetAuditLogAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/audit-logs?entityName=Applicant&entityId=" + Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAuditLogAsync_ShouldReturnForbidden_WhenAuthenticatedAsBroker()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var brokerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Broker);
        SetBearerToken(brokerToken);

        // Act
        var response = await Client.GetAsync("/api/audit-logs?entityName=Applicant&entityId=" + Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAuditLogAsync_ShouldReturnOk_WhenAuthenticatedAsAdmin()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var applicantId = await Client.CreateApplicantAsync();

        // Act
        var response = await Client.GetAsync($"/api/audit-logs?entityName=ApplicantEntity&entityId={applicantId}", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetAuditLogResponse>>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.Results.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAuditLogAsync_ShouldReturnOk_WhenAuthenticatedAsManager()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var managerToken = await Client.CreateAndLoginAsRoleAsync(OperatorRole.Manager);
        SetBearerToken(managerToken);

        // Act
        var response = await Client.GetAsync("/api/audit-logs?entityName=ApplicantEntity&entityId=" + Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAuditLogAsync_ShouldReturnBadRequest_WhenEntityNameIsInvalid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync($"/api/audit-logs?entityName=999&entityId={Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAuditLogAsync_ShouldReturnBadRequest_WhenEntityIdIsEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/audit-logs?entityName=Applicant&entityId=" + Guid.Empty, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}