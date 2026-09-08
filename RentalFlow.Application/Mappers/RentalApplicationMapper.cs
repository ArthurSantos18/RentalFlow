using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class RentalApplicationMapper
{
    public static RentalApplicationEntity ToEntity(this AddRentalApplicationRequest request,
        ApplicantEntity applicantEntity,
        PropertyEntity propertyEntity,
        OperatorEntity operatorEntity)
    {
        return RentalApplicationBuilder.Create()
            .WithFinancedAmount(request.FinancedAmount)
            .WithTotalAmount(request.TotalAmount)
            .WithInstallments(request.Installments)
            .WithContractDate(request.ContractDate ?? DateTime.UtcNow)
            .WithApplicant(applicantEntity)
            .WithProperty(propertyEntity)
            .WithOperator(operatorEntity)
            .Build();
    }

    public static RentalApplicationEntity UpdateEntity(this UpdateRentalApplicationRequest request, RentalApplicationEntity entity)
    {
        return entity
            .SetFinancedAmount(request.FinancedAmount ?? entity.FinancedAmount)
            .SetTotalAmount(request.TotalAmount ?? entity.TotalAmount)
            .SetInstallments(request.Installments ?? entity.Installments)
            .SetContractDate(request.ContractDate ?? entity.ContractDate);
    }

    public static GetRentalApplicationResponse ToResponse(this RentalApplicationEntity entity)
    {
        return new GetRentalApplicationResponse
        {
            Id = entity.Id,
            ProposalNumber = entity.ProposalNumber,
            FinancedAmount = entity.FinancedAmount,
            TotalAmount = entity.TotalAmount,
            Installments = entity.Installments,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            ContractDate = entity.ContractDate,
            IsActive = entity.IsActive,
            ApplicantId = entity.ApplicantId,
            ApplicantName = entity.Applicant?.FullName ?? string.Empty,
            ApplicantCpf = entity.Applicant?.Cpf ?? string.Empty,
            PropertyId = entity.PropertyId,
            PropertyAddress = entity.Property?.Address?.Street ?? string.Empty,
            PropertyRentPrice = entity.Property?.RentPrice ?? 0,
            OperatorId = entity.OperatorId,
            OperatorName = entity.Operator?.Name ?? string.Empty
        };
    }

    public static PagedResult<GetRentalApplicationResponse> ToResponse(this PagedResult<RentalApplicationEntity> pagedResult)
    {
        return new PagedResult<GetRentalApplicationResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(r => r.ToResponse())
        };
    }
}
