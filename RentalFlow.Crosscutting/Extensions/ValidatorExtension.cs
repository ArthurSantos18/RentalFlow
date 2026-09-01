using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using RentalFlow.Application.UseCases.Queries.Applicant;
using RentalFlow.Application.Validators.Applicant;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Requests;
using RentalFlow.Application.Validators.Property;
using RentalFlow.Application.Requests.Property;

namespace RentalFlow.Crosscutting.Extensions;

public static class ValidatorExtension
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(x => x.DisableDataAnnotationsValidation = true);
        services.AddScoped<IValidator<GetApplicantRequest>, GetApplicantRequestValidator>();
        services.AddScoped<IValidator<UpdateApplicantRequest>, UpdateApplicantRequestValidator>();
        services.AddScoped<IValidator<AddApplicantRequest>, AddApplicantRequestValidator>();

        services.AddScoped<IValidator<GetPropertyRequest>, GetPropertiesRequestValidator>();
        services.AddScoped<IValidator<UpdatePropertyRequest>, UpdatePropertyRequestValidator>();
        services.AddScoped<IValidator<AddPropertyRequest>, AddPropertyRequestValidator>();


        return services;
    }
}
