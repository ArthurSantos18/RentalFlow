#pragma warning disable IDE0060

namespace RentalFlow.API.Metadata;

public static class ReportMetadata
{
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static void GetDashboardAsync(CancellationToken cancellationToken) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(GetTopPropertiesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static void GetTopPropertiesAsync(GetTopPropertiesRequest request, CancellationToken cancellationToken) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(GetTopOperatorsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static void GetTopOperatorsAsync(GetTopOperatorsRequest request, CancellationToken cancellationToken) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(GetApplicationsByPeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static void GetApplicationsByPeriodAsync(GetApplicationsByPeriodRequest request, CancellationToken cancellationToken) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(GetConversionRateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public static void GetConversionRateAsync(GetConversionRateRequest request, CancellationToken cancellationToken) { }
}
