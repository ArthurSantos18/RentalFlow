using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Helpers;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class ApplicantMapper
{
    public static ApplicantEntity ToEntity(this AddApplicantRequest request)
    {
        return new ApplicantBuilder()
            .WithId(Guid.NewGuid())
            .WithFullName(request.FullName)
            .WithCpf(CpfValidator.Normalize(request.Cpf))
            .WithEmail(request.Email)
            .WithPhone(request.Phone)
            .WithMonthlyIncome(request.MonthlyIncome)
            .WithActive(true)
            .Build();
    }

    public static ApplicantEntity UpdateEntity(this UpdateApplicantRequest request, ApplicantEntity entity)
    {
        return entity
            .SetFullName(request.FullName ?? entity.FullName)
            .SetCpf(string.IsNullOrEmpty(request.Cpf) ? entity.Cpf : CpfValidator.Normalize(request.Cpf))
            .SetEmail(request.Email ?? entity.Email)
            .SetPhone(request.Phone ?? entity.Phone)
            .SetMonthlyIncome(request.MonthlyIncome ?? entity.MonthlyIncome);
    }

    public static GetApplicantResponse ToResponse(this ApplicantEntity applicant)
    {
        return new GetApplicantResponse
        {
            Id = applicant.Id,
            FullName = applicant.FullName,
            Cpf = applicant.Cpf,
            Email = applicant.Email,
            Phone = applicant.Phone,
            MonthlyIncome = applicant.MonthlyIncome,
            IsActive = applicant.IsActive
        };
    }

    public static PagedResult<GetApplicantResponse> ToResponse(this PagedResult<ApplicantEntity> pagedResult)
    {
        return new PagedResult<GetApplicantResponse>
        {
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalResults = pagedResult.TotalResults,
            Results = pagedResult.Results.Select(p => p.ToResponse())
        };
    }
}
