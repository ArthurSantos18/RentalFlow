using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Infrastructure.Data;
using RentalFlow.Infrastructure.Repositories;

namespace RentalFlow.Infrastructure.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IApplicantRepository, ApplicantRepository>();
        services.AddScoped<IOperatorRepository, OperatorRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IRentalApplicationRepository, RentalApplicationRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();

        return services;
    }
}
