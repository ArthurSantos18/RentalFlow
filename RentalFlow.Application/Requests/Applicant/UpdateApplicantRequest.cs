namespace RentalFlow.Application.Requests.Applicant;

public sealed record UpdateApplicantRequest
{
    [Description("The full name of the applicant.")]
    public string? FullName { get; init; }

    [Description("The email of the applicant.")]
    public string? Email { get; init; }

    [Description("The phone number of the applicant.")]
    public string? Phone { get; init; }

    [Description("The monthly income of the applicant.")]
    public decimal? MonthlyIncome { get; init; }

    [Description("Indicates if the applicant is active.")]
    public bool? IsActive { get; init; }
};