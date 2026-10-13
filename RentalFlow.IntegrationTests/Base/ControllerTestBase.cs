namespace RentalFlow.IntegrationTests.Base;

[Collection(nameof(ControllerTestCollection))]
public abstract class ControllerTestBase(WebApplicationFixture fixture)
{
    protected HttpClient Client { get; } = fixture.CreateClient();
    protected string AdminEmail => fixture.AdminEmail;
    protected string AdminPassword => fixture.AdminPassword;

    protected async Task<LoginResponse> LoginAsAdminAsync()
    {
        var request = new LoginRequest
        {
            Email = fixture.AdminEmail,
            Password = fixture.AdminPassword
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!;
    }

    protected async Task AuthenticateAsAdminAsync()
    {
        var login = await LoginAsAdminAsync();
        SetBearerToken(login.AccessToken);
    }

    protected void SetBearerToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected void ClearAuth()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}