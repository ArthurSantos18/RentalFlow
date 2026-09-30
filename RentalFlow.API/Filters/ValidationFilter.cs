using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RentalFlow.API.Filters;

public sealed class ValidationFilter(IServiceProvider _serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var argumentType = argument.GetType();

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator is null)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            ValidationResult validationResult = await validator.ValidateAsync(validationContext);

            if (validationResult.IsValid)
            {
                continue;
            }

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = context.HttpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        await next();
    }
}