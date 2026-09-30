using RentalFlow.API.Filters;

namespace RentalFlow.API.Extensions;

public static class AddApiServicesExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<ValidationFilter>();

        services.AddControllers(options =>
        {
            options.Filters.AddService<ValidationFilter>();
        });

        return services;
    }
}
