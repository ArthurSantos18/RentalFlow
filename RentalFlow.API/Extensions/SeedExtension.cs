namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class SeedExtension
{
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        await DatabaseSeeder.SeedAsync(host.Services);
        return host;
    }
}