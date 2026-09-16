namespace RentalFlow.Application.Mappers;

public static class RentalApplicationMapper
{
    public static RentalApplicationEntity ToEntity(this AddRentalApplicationRequest request, ApplicantEntity applicant, PropertyEntity property, OperatorEntity @operator)
    {
        return new RentalApplicationEntity(
            installments: request.Installments,
            financedAmount: request.FinancedAmount,
            totalAmount: request.TotalAmount,
            contractDate: request.ContractDate ?? DateTime.UtcNow,
            proposalNumber: string.Empty,
            applicant: applicant,
            property: property,
            @operator: @operator);
    }

    public static RentalApplicationEntity UpdateFrom(this RentalApplicationEntity entity, UpdateRentalApplicationRequest request)
    {
        if (request.FinancedAmount.HasValue)
        {
            entity.SetFinancedAmount(request.FinancedAmount.Value);
        }

        if (request.TotalAmount.HasValue)
        {
            entity.SetTotalAmount(request.TotalAmount.Value);
        }

        if (request.Installments.HasValue)
        {
            entity.SetInstallments(request.Installments.Value);
        }

        if (request.ContractDate.HasValue)
        {
            entity.SetContractDate(request.ContractDate.Value);
        }

        return entity.Touch();
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
            ContractDate = entity.ContractDate,
            IsActive = entity.IsActive,
            ApplicantId = entity.ApplicantId,
            ApplicantName = entity.Applicant.FullName,
            ApplicantCpf = entity.Applicant.Cpf,
            PropertyId = entity.PropertyId,
            PropertyAddress = entity.Property.Address.Street,
            PropertyRentPrice = entity.Property.RentPrice,
            OperatorId = entity.OperatorId,
            OperatorName = entity.Operator.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
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
