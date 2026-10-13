namespace RentalFlow.IntegrationTests.Controllers;

public sealed class AuthControllerTests(WebApplicationFixture fixture) : ControllerTestBase(fixture)
{
    [Fact]
    public async Task LoginAsync_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(AdminEmail);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = AdminEmail,
            Password = "WrongPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "notfound@rentalflow.com",
            Password = "AnyPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnBadRequest_WhenEmailFormatIsInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "not-a-valid-email",
            Password = "AnyPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnBadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = string.Empty,
            Password = "AnyPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnBadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = AdminEmail,
            Password = string.Empty
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnOk_WhenTokenIsValid()
    {
        // Arrange
        var login = await LoginAsAdminAsync();
        var request = new RefreshTokenRequest { RefreshToken = login.RefreshToken };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", request, TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBe(login.RefreshToken);
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        var request = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnBadRequest_WhenTokenIsEmpty()
    {
        // Arrange
        var request = new RefreshTokenRequest { RefreshToken = string.Empty };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnNoContent_WhenRefreshTokenIsValid()
    {
        // Arrange
        var login = await LoginAsAdminAsync();
        var request = new LogoutRequest { RefreshToken = login.RefreshToken };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/logout", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnNoContent_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var request = new LogoutRequest { RefreshToken = "invalid-refresh-token" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/logout", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        ClearAuth();

        var request = new ChangePasswordRequest
        {
            CurrentPassword = AdminPassword,
            NewPassword = "NewPassword@123",
            ConfirmNewPassword = "NewPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnUnauthorized_WhenCurrentPasswordIsWrong()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var request = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword@123",
            NewPassword = "NewPassword@123",
            ConfirmNewPassword = "NewPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnBadRequest_WhenNewPasswordEqualsCurrent()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var request = new ChangePasswordRequest
        {
            CurrentPassword = AdminPassword,
            NewPassword = AdminPassword,
            ConfirmNewPassword = AdminPassword
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnBadRequest_WhenConfirmPasswordDoesNotMatch()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var request = new ChangePasswordRequest
        {
            CurrentPassword = AdminPassword,
            NewPassword = "NewPassword@123",
            ConfirmNewPassword = "DifferentPassword@456"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnBadRequest_WhenNewPasswordIsWeak()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var request = new ChangePasswordRequest
        {
            CurrentPassword = AdminPassword,
            NewPassword = "abc",
            ConfirmNewPassword = "abc"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnNoContent_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = AdminPassword,
            NewPassword = "NewPassword@123",
            ConfirmNewPassword = "NewPassword@123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/change-password", changeRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Cleanup: restore original password
        var restoreRequest = new ChangePasswordRequest
        {
            CurrentPassword = "NewPassword@123",
            NewPassword = AdminPassword,
            ConfirmNewPassword = AdminPassword
        };

        await Client.PostAsJsonAsync("/api/auth/change-password", restoreRequest, TestContext.Current.CancellationToken);
    }
}