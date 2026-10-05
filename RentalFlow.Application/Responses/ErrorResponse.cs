namespace RentalFlow.Application.Responses;

public sealed record ErrorResponse
{
    [Description("The error code associated with the response.")]
    public int Code { get; init; }

    [Description("The error message associated with the response.")]
    public string Message { get; init; } = string.Empty;
}