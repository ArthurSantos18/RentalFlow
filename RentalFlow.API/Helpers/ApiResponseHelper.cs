namespace RentalFlow.API.Helpers;

public class ApiResponseHelper
{
    public static IActionResult HandleError(Error error)
    {
        return new ObjectResult(new
        {
            error.Code,
            error.Message
        })
        {
            StatusCode = error.Code
        };
    }
}
