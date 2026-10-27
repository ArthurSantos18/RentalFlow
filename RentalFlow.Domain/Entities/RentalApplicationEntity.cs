namespace RentalFlow.Domain.Entities;

public sealed class RentalApplicationEntity : BaseEntity<RentalApplicationEntity>
{
    public Guid ApplicantId { get; private set; }

    public Guid PropertyId { get; private set; }

    public Guid OperatorId { get; private set; }

    public int Installments { get; private set; }

    public decimal FinancedAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public RentalStatus Status { get; private set; } = RentalStatus.Draft;

    public DateTime ContractDate { get; private set; }

    public string ProposalNumber { get; private set; } = string.Empty;

    public ApplicantEntity Applicant { get; private set; } = null!;

    public PropertyEntity Property { get; private set; } = null!;

    public OperatorEntity Operator { get; private set; } = null!;

    public RentalApplicationEntity(
        int installments,
        decimal financedAmount,
        decimal totalAmount,
        DateTime contractDate,
        string proposalNumber,
        ApplicantEntity applicant,
        PropertyEntity property,
        OperatorEntity @operator)
    {
        ApplicantId = applicant.Id;
        PropertyId = property.Id;
        OperatorId = @operator.Id;
        Installments = installments;
        FinancedAmount = financedAmount;
        TotalAmount = totalAmount;
        ContractDate = contractDate;
        ProposalNumber = string.IsNullOrEmpty(proposalNumber) ? GenerateProposalNumber() : proposalNumber;
        Applicant = applicant;
        Property = property;
        Operator = @operator;
    }

    private RentalApplicationEntity() { }

    public RentalApplicationEntity SetInstallments(int installments)
    {
        Installments = installments;
        return this;
    }

    public RentalApplicationEntity SetFinancedAmount(decimal financedAmount)
    {
        FinancedAmount = financedAmount;
        return this;
    }

    public RentalApplicationEntity SetTotalAmount(decimal totalAmount)
    {
        TotalAmount = totalAmount;
        return this;
    }

    public RentalApplicationEntity SetStatus(RentalStatus status)
    {
        Status = status;
        return this;
    }

    public RentalApplicationEntity SetContractDate(DateTime contractDate)
    {
        ContractDate = contractDate;
        return this;
    }

    public RentalApplicationEntity SetProposalNumber(string proposalNumber)
    {
        ProposalNumber = proposalNumber;
        return this;
    }

    public bool CanBeEdited() => Status is RentalStatus.Draft or RentalStatus.Pending;

    public bool IsApplicantAssigned(Guid applicantId) => ApplicantId == applicantId;

    public bool IsOperatorAssigned(Guid operatorId) => OperatorId == operatorId;

    public bool IsPropertyAssigned(Guid propertyId) => PropertyId == propertyId;

    public bool CanTransitionTo(RentalStatus newStatus) =>
        (Status, newStatus) switch
        {
            (RentalStatus.Draft, RentalStatus.Pending) => true,
            (RentalStatus.Pending, RentalStatus.Draft) => true,
            (RentalStatus.Pending, RentalStatus.Approved) => true,
            (RentalStatus.Pending, RentalStatus.Rejected) => true,
            _ => false
        };

    public void ChangeApplicant(ApplicantEntity applicant)
    {
        Applicant = applicant;
        ApplicantId = applicant.Id;
    }

    public void ChangeOperator(OperatorEntity @operator)
    {
        Operator = @operator;
        OperatorId = @operator.Id;
    }

    public void ChangeProperty(PropertyEntity property)
    {
        Property = property;
        PropertyId = property.Id;
    }

    public bool TryChangeStatus(RentalStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
        {
            return false;
        }

        Status = newStatus;
        return true;
    }

    internal static string GenerateProposalNumber() => $"PRO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}