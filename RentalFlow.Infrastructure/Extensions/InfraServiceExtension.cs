namespace RentalFlow.Infrastructure.Extensions;

public static class InfraServiceExtension
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordService, BCryptPasswordService>();

        return services;
    }
}
