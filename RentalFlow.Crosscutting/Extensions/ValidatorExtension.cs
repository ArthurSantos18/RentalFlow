namespace RentalFlow.Crosscutting.Extensions;

public static class ValidatorExtension
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AddApplicantRequestValidator>();

        return services;
    }
}