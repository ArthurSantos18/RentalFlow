namespace RentalFlow.Crosscutting.Extensions;

public static class LiteBusExtension
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddLiteBus(liteBus =>
        {
            liteBus.AddMessaging(_ => { } );
            liteBus.AddCommands(module =>
            {
                module.RegisterFromAssembly(typeof(AddApplicantCommand).Assembly);
            });

            liteBus.AddQueries(module =>
            {
                module.RegisterFromAssembly(typeof(GetApplicantsQuery).Assembly);
            });
        });

        return services;
    }
}
