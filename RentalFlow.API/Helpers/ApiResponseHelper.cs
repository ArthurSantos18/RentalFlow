using Microsoft.AspNetCore.Mvc;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.API.Helpers;

public class ApiResponseHelper
{
    public static IActionResult HandleError(Error error)
    {
        return error.Code switch
        {
            400 => new BadRequestObjectResult(new { error.Code, error.Message }),
            401 => new UnauthorizedObjectResult(new { error.Code, error.Message }),
            404 => new NotFoundObjectResult(new { error.Code, error.Message }),
            409 => new ConflictObjectResult(new { error.Code, error.Message }),
            500 => new ObjectResult(new { error.Code, error.Message })
            { StatusCode = 500 },
            _ => new BadRequestObjectResult(new { error.Code, error.Message })
        };
    }
}
