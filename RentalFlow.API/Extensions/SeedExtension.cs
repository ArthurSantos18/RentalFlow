using Microsoft.EntityFrameworkCore;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.API.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extension class for adding API services.")]
public static class SeedExtension
{
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
        }

        await DatabaseSeeder.SeedAsync(host.Services);

        return host;
    }
}