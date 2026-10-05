namespace RentalFlow.Application.Requests.Applicant;

public sealed record AddApplicantRequest
{
    [Description("The full name of the applicant.")]
    public string FullName { get; init; } = string.Empty;

    [Description("The CPF of the applicant.")]
    public string Cpf { get; init; } = string.Empty;

    [Description("The email of the applicant.")]
    public string Email { get; init; } = string.Empty;

    [Description("The phone number of the applicant.")]
    public string Phone { get; init; } = string.Empty;

    [Description("The monthly income of the applicant.")]
    public decimal MonthlyIncome { get; init; }
}