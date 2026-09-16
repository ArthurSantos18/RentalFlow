using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentalFlow.Application.Interfaces.Services;
using RentalFlow.Infrastructure.Services;

namespace RentalFlow.Crosscutting.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordService, BCryptPasswordService>();

        return services;
    }
}
