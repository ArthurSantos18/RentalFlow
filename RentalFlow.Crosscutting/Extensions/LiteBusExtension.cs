using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Messaging;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Application.UseCases.Queries.Applicant;

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
