using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalFlow.Infrastructure.Settings;

namespace RentalFlow.Crosscutting.Extensions;

public static class SettingsExtension
{
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        return services;
    }
}
