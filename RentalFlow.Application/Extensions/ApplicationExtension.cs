namespace RentalFlow.Application.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDataScopeService, DataScopeService>();

        return services;
    }
}