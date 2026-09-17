namespace RentalFlow.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var settings = serviceProvider.GetRequiredService<IOptions<SeedSettings>>().Value;

        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var operatorRepository = serviceProvider.GetRequiredService<IOperatorRepository>();
        var teamRepository = serviceProvider.GetRequiredService<ITeamRepository>();
        var passwordService = serviceProvider.GetRequiredService<IPasswordService>();

        if (string.IsNullOrWhiteSpace(settings.AdminEmail) || string.IsNullOrWhiteSpace(settings.AdminPassword))
        {
            throw new InvalidOperationException("Seed credentials are not configured.");
        }

        var existingUser = await userRepository.GetByEmailAsync(settings.AdminEmail);

        if (existingUser is not null)
        {
            return;
        }

        var adminTeam = new TeamEntity(
            name: settings.TeamName,
            description: "Time responsável pela administração do sistema"
        );

        await teamRepository.AddAsync(adminTeam, CancellationToken.None);

        var adminOperator = new OperatorEntity(
            name: settings.AdminName,
            role: OperatorRole.Administrator,
            team: adminTeam
        );

        await operatorRepository.AddAsync(adminOperator, CancellationToken.None);

        var passwordHash = passwordService.Hash(settings.AdminPassword);

        var adminUser = new UserEntity(
            email: settings.AdminEmail,
            passwordHash: passwordHash,
            mustChangePassword: false,
            @operator: adminOperator
        );

        await userRepository.AddAsync(adminUser, CancellationToken.None);

        await userRepository.SaveChangesAsync(CancellationToken.None);
    }
}
