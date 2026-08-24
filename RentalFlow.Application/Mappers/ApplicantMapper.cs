using RentalFlow.Application.Requests;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Entities.Applicant;
using System.Security.Cryptography;

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

    public static ApplicantUpdate ToDomain(this UpdateApplicantRequest request)
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
            MonthlyIncome = applicant.MonthlyIncome
        };
    }

    public static IEnumerable<GetApplicantResponse> ToResponse(this IEnumerable<ApplicantEntity> applicants)
    {
        return applicants.Select(a => a.ToResponse());
    }
}
