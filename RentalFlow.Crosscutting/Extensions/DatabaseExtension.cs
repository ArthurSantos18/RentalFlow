namespace RentalFlow.Crosscutting.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        await DatabaseSeeder.SeedAsync(host.Services);
        return host;
    }
}
