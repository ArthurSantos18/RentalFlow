namespace RentalFlow.IntegrationTests.Helpers;

public static class ApiSetup
{
    public static async Task<Guid> CreateApplicantAsync(this HttpClient client, string? cpf = null, string? email = null)
    {
        var request = Builders.ApplicantRequest(cpf: cpf, email: email);

        var response = await client.PostAsJsonAsync("/api/applicants", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    public static async Task<Guid> CreatePropertyAsync(this HttpClient client, decimal rentPrice = 2500m, int bedrooms = 2, bool isAvailable = true)
    {
        var request = Builders.PropertyRequest(rentPrice: rentPrice, bedrooms: bedrooms, isAvailable: isAvailable);

        var response = await client.PostAsJsonAsync("/api/properties", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    public static async Task<Guid> CreateTeamAsync(this HttpClient client, string? name = null, string? description = null)
    {
        var request = Builders.TeamRequest(name: name, description: description);

        var response = await client.PostAsJsonAsync("/api/teams", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    public static async Task<AddOperatorResponse> CreateOperatorAsync(this HttpClient client, Guid teamId, string? name = null, string? email = null, OperatorRole role = OperatorRole.Broker)
    {
        var request = Builders.OperatorRequest(teamId, name, email, role);

        var response = await client.PostAsJsonAsync("/api/operators", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AddOperatorResponse>(TestContext.Current.CancellationToken))!;
    }

    public static async Task<string> CreateAndLoginAsRoleAsync(this HttpClient client, OperatorRole role, Guid? teamId = null)
    {
        teamId ??= await client.CreateTeamAsync();

        var created = await client.CreateOperatorAsync(teamId.Value, role: role);

        var request = new LoginRequest
        {
            Email = created.Email,
            Password = created.TemporaryPassword
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

        if (login!.MustChangePassword)
        {
            var newPassword = "NewPassword@123";

            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = created.TemporaryPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = newPassword
            };


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

            await client.PostAsJsonAsync("/api/auth/change-password", changePasswordRequest, TestContext.Current.CancellationToken);

            var newLoginRequest = new LoginRequest
            {
                Email = created.Email,
                Password = newPassword
            };

            var newLoginResponse = await client.PostAsJsonAsync("/api/auth/login", newLoginRequest, TestContext.Current.CancellationToken);

            login = await newLoginResponse.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        }

        return login!.AccessToken;
    }

    public static async Task<Guid> CreateRentalApplicationAsync(this HttpClient client, Guid? applicantId = null, Guid? propertyId = null, Guid? operatorId = null)
    {
        applicantId ??= await client.CreateApplicantAsync();
        propertyId ??= await client.CreatePropertyAsync();

        var request = Builders.RentalApplicationRequest(applicantId.Value, propertyId.Value, operatorId);

        var response = await client.PostAsJsonAsync("/api/rental-applications", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
    }

    public static async Task ChangeRentalApplicationStatusAsync(this HttpClient client, Guid id, RentalStatus status)
    {
        var request = new UpdateRentalApplicationStatusRequest { RentalStatus = status };

        var response = await client.PatchAsJsonAsync($"/api/rental-applications/{id}/status", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }
}