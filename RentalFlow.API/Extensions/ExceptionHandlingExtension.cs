namespace RentalFlow.API.Extensions;

public static class ExceptionHandlingExtension
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}