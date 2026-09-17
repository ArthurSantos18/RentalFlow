namespace RentalFlow.Crosscutting.Extensions;

public static class SettingsExtension
{
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<SeedSettings>(configuration.GetSection("Seed"));

        return services;
    }
}
