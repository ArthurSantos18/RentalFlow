using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Application.Mappers;

public static class ApplicantMapper
{
    public static ApplicantEntity ToEntity(this AddApplicantRequest request)
    {
        return new ApplicantBuilder()
            .WithId(Guid.NewGuid())
            .WithFullName(request.FullName)
            .WithCpf(request.Cpf)
            .WithEmail(request.Email)
            .WithPhone(request.Phone)
            .WithMonthlyIncome(request.MonthlyIncome)
            .WithActive(true)
            .Build();
    }

    public static ApplicantUpdate ToUpdateDomain(this UpdateApplicantRequest request)
    {
        return new ApplicantUpdate(
            request.FullName,
            request.Cpf,
            request.Email,
            request.Phone,
            request.MonthlyIncome);
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
