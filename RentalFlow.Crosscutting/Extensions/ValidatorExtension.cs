using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using RentalFlow.Application.UseCases.Queries.Applicant;
using RentalFlow.Application.Requests;
using RentalFlow.Application.Validators;

namespace RentalFlow.Crosscutting.Extensions;

public static class ValidatorExtension
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(x => x.DisableDataAnnotationsValidation = true);
        services.AddScoped<IValidator<GetApplicantByCpfRequest>, GetApplicantByCpfRequestValidator>();
        services.AddScoped<IValidator<UpdateApplicantRequest>, UpdateApplicantRequestValidator>();
        services.AddScoped<IValidator<AddApplicantRequest>, AddApplicantRequestValidator>();

        return services;
    }
}
