namespace RentalFlow.Application.Requests.Applicant;

public sealed record UpdateApplicantRequest
{
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public decimal? MonthlyIncome { get; init; }
    public bool? IsActive { get; init; }
};
