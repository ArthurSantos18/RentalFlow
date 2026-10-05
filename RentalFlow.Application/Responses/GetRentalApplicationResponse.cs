namespace RentalFlow.Application.Responses;

public sealed record GetRentalApplicationResponse : BaseResponse
{
    [Description("The proposal number of the rental application.")]
    public string ProposalNumber { get; init; } = string.Empty;

    [Description("The amount financed for the rental application.")]
    public decimal FinancedAmount { get; init; }

    [Description("The total amount for the rental application.")]
    public decimal TotalAmount { get; init; }

    [Description("The number of installments for the rental application.")]
    public int Installments { get; init; }

    [Description("The status of the rental application.")]
    public RentalStatus Status { get; init; }

    [Description("The date of the rental contract.")]
    public DateTime? ContractDate { get; init; }

    [Description("The unique identifier of the applicant.")]
    public Guid ApplicantId { get; init; }

    [Description("The name of the applicant.")]
    public string ApplicantName { get; init; } = string.Empty;

    [Description("The CPF of the applicant.")]
    public string ApplicantCpf { get; init; } = string.Empty;

    [Description("The unique identifier of the property.")]
    public Guid PropertyId { get; init; }

    [Description("The address of the property.")]
    public string PropertyAddress { get; init; } = string.Empty;

    [Description("The rent price of the property.")]
    public decimal PropertyRentPrice { get; init; }

    [Description("The unique identifier of the operator.")]
    public Guid OperatorId { get; init; }

    [Description("The name of the operator.")]
    public string OperatorName { get; init; } = string.Empty;
}