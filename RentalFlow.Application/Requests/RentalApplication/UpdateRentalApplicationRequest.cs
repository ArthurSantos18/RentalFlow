namespace RentalFlow.Application.Requests.RentalApplication;

public sealed record UpdateRentalApplicationRequest
{
    [Description("The number of installments for the rental application.")]
    public int? Installments { get; init; }

    [Description("The amount financed for the rental application.")]
    public decimal? FinancedAmount { get; init; }

    [Description("The total amount for the rental application.")]
    public decimal? TotalAmount { get; init; }

    [Description("The date of the rental contract.")]
    public DateTime? ContractDate { get; init; }

    [Description("The ID of the applicant.")]
    public Guid? ApplicantId { get; init; }

    [Description("The ID of the operator.")]
    public Guid? OperatorId { get; init; }

    [Description("The ID of the property.")]
    public Guid? PropertyId { get; init; }
}