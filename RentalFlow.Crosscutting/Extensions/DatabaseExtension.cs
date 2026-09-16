using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Crosscutting.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
