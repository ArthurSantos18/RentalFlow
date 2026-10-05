namespace RentalFlow.API.Helpers;

public class ApiResponseHelper
{
    public static IActionResult HandleError(Error error)
    {
        return new ObjectResult(new ErrorResponse
        {
            Code = error.Code,
            Message = error.Message
        })
        {
            StatusCode = error.Code
        };
    }
}