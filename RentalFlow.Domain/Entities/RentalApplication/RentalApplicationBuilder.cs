using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Enums;

namespace RentalFlow.Domain.Entities.RentalApplication;

public sealed class RentalApplicationBuilder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicantId { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; } = Guid.NewGuid();
    public Guid OperatorId { get; set; } = Guid.NewGuid();
    public Guid Installments { get; set; } = Guid.NewGuid();
    public decimal FinancedAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ContractDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string ProposalNumber { get; set; } = string.Empty;
    public ApplicantEntity Applicant { get; set; } = ApplicantEntity.Empty;
    public PropertyEntity Property { get; set; } = PropertyEntity.Empty;
    public OperatorEntity Operator { get; set; } = OperatorEntity.Empty;

    public static RentalApplicationBuilder Create() => new();

    public RentalApplicationBuilder WithId(Guid id) { Id = id; return this; }

    public RentalApplicationBuilder WithApplicantId(Guid applicantId) { ApplicantId = applicantId; return this; }

    public RentalApplicationBuilder WithPropertyId(Guid propertyId) { PropertyId = propertyId; return this; }

    public RentalApplicationBuilder WithOperatorId(Guid operatorId) { OperatorId = operatorId; return this; }

    public RentalApplicationBuilder WithInstallments(Guid installments) { Installments = installments; return this; }

    public RentalApplicationBuilder WithFinancedAmount(decimal financedAmount) { FinancedAmount = financedAmount; return this; }

    public RentalApplicationBuilder WithTotalAmount(decimal totalAmount) { TotalAmount = totalAmount; return this; }

    public RentalApplicationBuilder WithStatus(RentalStatus status) { Status = status; return this; }

    public RentalApplicationBuilder WithCreatedAt(DateTime createdAt) { CreatedAt = createdAt; return this; }

    public RentalApplicationBuilder WithContractDate(DateTime contractDate) { ContractDate = contractDate; return this; }

    public RentalApplicationBuilder WithIsActive(bool isActive) { IsActive = isActive; return this; }

    public RentalApplicationBuilder WithProposalNumber(string proposalNumber) { ProposalNumber = proposalNumber; return this; }

    public RentalApplicationBuilder WithApplicant(ApplicantEntity applicant) { Applicant = applicant; return this; }

    public RentalApplicationBuilder WithProperty(PropertyEntity property) { Property = property; return this; }

    public RentalApplicationBuilder WithOperator(OperatorEntity @operator) { Operator = @operator; return this; }


    public RentalApplicationEntity Build()
    {

        return new RentalApplicationEntity(
            Id,
            ApplicantId,
            PropertyId,
            OperatorId,
            Installments,
            FinancedAmount,
            TotalAmount,
            Status,
            CreatedAt,
            ContractDate,
            IsActive,
            ProposalNumber,
            Applicant,
            Property,
            Operator
        );
    }
}