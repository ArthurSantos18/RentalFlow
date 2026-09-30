namespace RentalFlow.API.Extensions;

public static class SeedExtension
{
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        await DatabaseSeeder.SeedAsync(host.Services);
        return host;
    }
}