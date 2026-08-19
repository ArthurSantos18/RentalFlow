using RentalFlow.Domain.Entities.RentalApplication;

namespace RentalFlow.Domain.Entities.Applicant;

public sealed class ApplicantBuilder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public List<RentalApplicationEntity> Applications { get; set; } = [];

    public static ApplicantBuilder Create() => new();

    public ApplicantBuilder WithId(Guid id) { Id = id; return this; }

    public ApplicantBuilder WithFullName(string fullName) { FullName = fullName; return this; }

    public ApplicantBuilder WithCpf(string cpf) { Cpf = cpf; return this; }

    public ApplicantBuilder WithEmail(string email) { Email = email; return this; }

    public ApplicantBuilder WithPhone(string phone) { Phone = phone; return this; }

    public ApplicantBuilder WithMonthlyIncome(decimal monthlyIncome) { MonthlyIncome = monthlyIncome; return this; }

    public ApplicantBuilder WithApplications(List<RentalApplicationEntity> applications) { Applications = applications; return this; }

    public ApplicantBuilder AddApplication(RentalApplicationEntity application)
    {
        Applications.Add(application);
        return this;
    }

    public ApplicantEntity Build()
    {
        return new ApplicantEntity(
            Id,
            FullName,
            Cpf,
            Email,
            Phone,
            MonthlyIncome,
            Applications
        );
    }
}