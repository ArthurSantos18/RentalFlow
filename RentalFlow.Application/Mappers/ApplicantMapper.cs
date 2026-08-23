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

    public static ApplicantEntity ApplyUpdates(this ApplicantEntity oldApplicant, UpdateApplicantRequest request)
    {
        if (!string.IsNullOrEmpty(request.FullName))
            oldApplicant.SetFullName(request.FullName);

        if (!string.IsNullOrEmpty(request.Cpf))
            oldApplicant.SetCpf(request.Cpf);

        if (!string.IsNullOrEmpty(request.Email))
            oldApplicant.SetEmail(request.Email);

        if (!string.IsNullOrEmpty(request.Phone))
            oldApplicant.SetPhone(request.Phone);

        if (request.MonthlyIncome.HasValue)
            oldApplicant.SetMonthlyIncome(request.MonthlyIncome.Value);

        return oldApplicant;
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
