namespace RentalFlow.Crosscutting.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration.GetConnectionString("DefaultConnection")!);
        services.AddRepositories();
        services.AddSettings(configuration);
        services.AddServices();

        return services;
    }
}
