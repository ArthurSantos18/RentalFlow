using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.RentalApplication;
public sealed class RentalApplicationEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ApplicantId { get; private set; } = Guid.NewGuid();
    public Guid PropertyId { get; private set; } = Guid.NewGuid();
    public Guid OperatorId { get; private set; } = Guid.NewGuid();
    public Guid Installments { get; private set; } = Guid.NewGuid();
    public decimal FinancedAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public RentalStatus Status { get; private set; } = RentalStatus.Draft;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime ContractDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string ProposalNumber { get; private set; } = string.Empty;
    public ApplicantEntity Applicant { get; private set; } = ApplicantEntity.Empty;
    public PropertyEntity Property { get; private set; } = PropertyEntity.Empty;
    public OperatorEntity Operator { get; private set; } = OperatorEntity.Empty;

    public RentalApplicationEntity(
        Guid id,
        Guid applicantId,
        Guid propertyId,
        Guid operatorId,
        Guid installments,
        decimal financedAmount,
        decimal totalAmount,
        RentalStatus status,
        DateTime createdAt,
        DateTime contractDate,
        bool isActive,
        string proposalNumber,
        ApplicantEntity applicant,
        PropertyEntity property,
        OperatorEntity @operator)
    {
        Id = id;
        ApplicantId = applicantId;
        PropertyId = propertyId;
        OperatorId = operatorId;
        Installments = installments;
        FinancedAmount = financedAmount;
        TotalAmount = totalAmount;
        Status = status;
        CreatedAt = createdAt;
        ContractDate = contractDate;
        IsActive = isActive;
        ProposalNumber = proposalNumber;
        Applicant = applicant;
        Property = property;
        Operator = @operator;
    }

    private RentalApplicationEntity() { }

    public RentalApplicationEntity SetId(Guid id) { Id = id; return this; }
    public RentalApplicationEntity SetApplicantId(Guid applicantId) { ApplicantId = applicantId; return this; }
    public RentalApplicationEntity SetPropertyId(Guid propertyId) { PropertyId = propertyId; return this; }
    public RentalApplicationEntity SetOperatorId(Guid operatorId) { OperatorId = operatorId; return this; }
    public RentalApplicationEntity SetInstallments(Guid installments) { Installments = installments; return this; }
    public RentalApplicationEntity SetFinancedAmount(decimal financedAmount) { FinancedAmount = financedAmount; return this; }
    public RentalApplicationEntity SetTotalAmount(decimal totalAmount) { TotalAmount = totalAmount; return this; }
    public RentalApplicationEntity SetStatus(RentalStatus status) { Status = status; return this; }
    public RentalApplicationEntity SetCreatedAt(DateTime createdAt) { CreatedAt = createdAt; return this; }
    public RentalApplicationEntity SetContractDate(DateTime contractDate) { ContractDate = contractDate; return this; }
    public RentalApplicationEntity SetIsActive(bool isActive) { IsActive = isActive; return this; }
    public RentalApplicationEntity SetProposalNumber(string proposalNumber) { ProposalNumber = proposalNumber; return this; }
    public RentalApplicationEntity SetApplicant(ApplicantEntity applicant) { Applicant = applicant; return this; }
    public RentalApplicationEntity SetProperty(PropertyEntity property) { Property = property; return this; }
    public RentalApplicationEntity SetOperator(OperatorEntity @operator) { Operator = @operator; return this; }

    public RentalApplicationBuilder ToBuilder() => new()
    {
        Id = Id,
        ApplicantId = ApplicantId,
        PropertyId = PropertyId,
        OperatorId = OperatorId,
        Installments = Installments,
        FinancedAmount = FinancedAmount,
        TotalAmount = TotalAmount,
        Status = Status,
        CreatedAt = CreatedAt,
        ContractDate = ContractDate,
        IsActive = IsActive,
        ProposalNumber = ProposalNumber,
        Applicant = Applicant,
        Property = Property,
        Operator = Operator
    };
}
