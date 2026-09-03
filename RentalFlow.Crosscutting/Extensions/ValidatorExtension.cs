using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Validators.RentalApplication;
using RentalFlow.Application.Validators.Applicant;
using RentalFlow.Application.Validators.Operator;
using RentalFlow.Application.Validators.Property;

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

        services.AddScoped<IValidator<GetOperatorRequest>, GetOperatorsRequestValidator>();
        services.AddScoped<IValidator<UpdateOperatorRequest>, UpdateOperatorRequestValidator>();
        services.AddScoped<IValidator<AddOperatorRequest>, AddOperatorRequestValidator>();

        services.AddScoped<IValidator<AddRentalApplicationRequest>, AddRentalApplicationRequestValidator>();
        services.AddScoped<IValidator<UpdateRentalApplicationRequest>, UpdateRentalApplicationRequestValidator>();
        services.AddScoped<IValidator<GetRentalApplicationRequest>, GetRentalApplicationRequestValidator>();

        return services;
    }
}
