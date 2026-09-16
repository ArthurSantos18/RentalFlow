namespace RentalFlow.Application.Mappers;

public static class ApplicantMapper
{
    public static ApplicantEntity ToEntity(this AddApplicantRequest request)
    {
        return new ApplicantEntity(
            fullName: request.FullName,
            cpf: CpfValidator.Normalize(request.Cpf),
            email: request.Email,
            phone: request.Phone,
            monthlyIncome: request.MonthlyIncome);
    }

    public static ApplicantEntity UpdateFrom(this ApplicantEntity entity, UpdateApplicantRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            entity.SetFullName(request.FullName);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            entity.SetEmail(request.Email);
        }

        if (request.Phone is not null)
        {
            entity.SetPhone(string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone);
        }

        if (request.MonthlyIncome.HasValue)
        {
            entity.SetMonthlyIncome(request.MonthlyIncome.Value);
        }

        if (request.IsActive.HasValue)
        {
            entity.SetIsActive(request.IsActive.Value);
        }

        return entity.Touch();
    }

    public static GetApplicantResponse ToResponse(this ApplicantEntity entity)
    {
        return new GetApplicantResponse
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Cpf = entity.Cpf,
            Email = entity.Email,
            Phone = entity.Phone,
            MonthlyIncome = entity.MonthlyIncome,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static PagedResult<GetApplicantResponse> ToResponse(this PagedResult<ApplicantEntity> pagedResult)
    {
        return new PagedResult<GetApplicantResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(a => a.ToResponse())
        };
    }
}
