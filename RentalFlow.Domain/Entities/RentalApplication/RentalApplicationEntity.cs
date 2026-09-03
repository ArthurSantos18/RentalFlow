using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.RentalApplication;

public sealed class RentalApplicationEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ApplicantId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid OperatorId { get; private set; }
    public int Installments { get; private set; }
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
        int installments,
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
        ApplicantId = applicant.Id;
        PropertyId = property.Id;
        OperatorId = @operator.Id;
        Installments = installments;
        FinancedAmount = financedAmount;
        TotalAmount = totalAmount;
        Status = status;
        CreatedAt = createdAt;
        ContractDate = contractDate;
        IsActive = isActive;
        ProposalNumber = string.IsNullOrEmpty(proposalNumber) ? GenerateProposalNumber() : proposalNumber;
        Applicant = applicant;
        Property = property;
        Operator = @operator;
    }

    public static RentalApplicationEntity Empty { get; } = new RentalApplicationEntity
    {
        Id = Guid.NewGuid(),
        Installments = 0,
        FinancedAmount = 0,
        TotalAmount = 0,
        Status = RentalStatus.Draft,
        CreatedAt = DateTime.UtcNow,
        ContractDate = DateTime.MinValue,
        IsActive = false,
        ProposalNumber = string.Empty,
        Applicant = ApplicantEntity.Empty,
        Property = PropertyEntity.Empty,
        Operator = OperatorEntity.Empty
    };

    private RentalApplicationEntity() { }

    public RentalApplicationEntity SetId(Guid id) { Id = id; return this; }
    public RentalApplicationEntity SetInstallments(int installments) { Installments = installments; return this; }
    public RentalApplicationEntity SetFinancedAmount(decimal financedAmount) { FinancedAmount = financedAmount; return this; }
    public RentalApplicationEntity SetTotalAmount(decimal totalAmount) { TotalAmount = totalAmount; return this; }
    public RentalApplicationEntity SetStatus(RentalStatus status) { Status = status; return this; }
    public RentalApplicationEntity SetCreatedAt(DateTime createdAt) { CreatedAt = createdAt; return this; }
    public RentalApplicationEntity SetContractDate(DateTime contractDate) { ContractDate = contractDate; return this; }
    public RentalApplicationEntity SetIsActive(bool isActive) { IsActive = isActive; return this; }
    public RentalApplicationEntity SetProposalNumber(string proposalNumber) { ProposalNumber = proposalNumber; return this; }
    public RentalApplicationEntity SetApplicant(ApplicantEntity applicant)
    {
        Applicant = applicant;
        ApplicantId = applicant?.Id ?? Guid.Empty;
        return this;
    }

    public RentalApplicationEntity SetProperty(PropertyEntity property)
    {
        Property = property;
        PropertyId = property?.Id ?? Guid.Empty;
        return this;
    }

    public RentalApplicationEntity SetOperator(OperatorEntity @operator)
    {
        Operator = @operator;
        OperatorId = @operator?.Id ?? Guid.Empty;
        return this;
    }

    public void Update(RentalApplicationUpdate update)
    {
        if (update.FinancedAmount.HasValue)
            SetFinancedAmount(update.FinancedAmount.Value);

        if (update.TotalAmount.HasValue)
            SetTotalAmount(update.TotalAmount.Value);

        if (update.Installments.HasValue)
            SetInstallments(update.Installments.Value);

        if (update.Status.HasValue)
            SetStatus(update.Status.Value);

        if (update.ContractDate.HasValue)
            SetContractDate(update.ContractDate.Value);

        if (update.IsActive.HasValue)
            SetIsActive(update.IsActive.Value);
    }

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

    internal static string GenerateProposalNumber() => $"PRO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}
