namespace RentalFlow.IntegrationTests.Controllers;

public sealed class PropertyControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task GetPropertiesAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.GetAsync("/api/properties", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPropertiesAsync_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.GetAsync("/api/properties", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GetPropertyResponse>>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.Results.Should().NotBeNull();
    }

    [Fact]
    public async Task AddPropertyAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = Builders.PropertyRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/properties", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddPropertyAsync_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.PropertyRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/properties", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<Guid>(
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddPropertyAsync_ShouldReturnBadRequest_WhenAddressIsEmpty()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new AddPropertyRequest
        {
            Address = Address.Empty,
            RentPrice = 2500m,
            Bedrooms = 2,
            IsAvailable = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/properties", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddPropertyAsync_ShouldReturnBadRequest_WhenRentPriceIsNegative()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.PropertyRequest(rentPrice: -100m);

        // Act
        var response = await Client.PostAsJsonAsync("/api/properties", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddPropertyAsync_ShouldReturnBadRequest_WhenBedroomsIsNegative()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = Builders.PropertyRequest(bedrooms: -1);

        // Act
        var response = await Client.PostAsJsonAsync("/api/properties", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();
        var request = new UpdatePropertyRequest { RentPrice = 3000m };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/properties/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var propertyId = await Client.CreatePropertyAsync();

        var request = new UpdatePropertyRequest
        {
            RentPrice = 3500m,
            Bedrooms = 3,
            IsAvailable = false
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/properties/{propertyId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ShouldReturnNoContent_WhenUpdatingAddress()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var propertyId = await Client.CreatePropertyAsync();

        var request = new UpdatePropertyRequest
        {
            Address = TestData.GenerateAddress(city: "Rio de Janeiro", state: "RJ")
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/properties/{propertyId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new UpdatePropertyRequest { RentPrice = 3000m };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/properties/{Guid.NewGuid()}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePropertyAsync_ShouldReturnBadRequest_WhenRentPriceIsNegative()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var propertyId = await Client.CreatePropertyAsync();

        var request = new UpdatePropertyRequest { RentPrice = -100m };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/properties/{propertyId}", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeletePropertyAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        // Act
        var response = await Client.DeleteAsync($"/api/properties/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeletePropertyAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var propertyId = await Client.CreatePropertyAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/properties/{propertyId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePropertyAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await Client.DeleteAsync($"/api/properties/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}