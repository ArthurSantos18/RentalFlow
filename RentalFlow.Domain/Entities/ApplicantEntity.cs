namespace RentalFlow.Domain.Entities;

public sealed class ApplicantEntity : BaseEntity<ApplicantEntity>
{
    public string FullName { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public decimal MonthlyIncome { get; private set; }
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public ApplicantEntity(
        string fullName,
        string cpf,
        string email,
        string? phone,
        decimal monthlyIncome)
    {
        FullName = fullName;
        Cpf = cpf;
        Email = email;
        Phone = phone;
        MonthlyIncome = monthlyIncome;
    }

    private ApplicantEntity() { }

    public ApplicantEntity SetFullName(string fullName)
    {
        FullName = fullName;
        return this;
    }

    public ApplicantEntity SetCpf(string cpf)
    {
        Cpf = cpf;
        return this;
    }

    public ApplicantEntity SetEmail(string email)
    {
        Email = email;
        return this;
    }

    public ApplicantEntity SetPhone(string? phone)
    {
        Phone = phone;
        return this;
    }

    public ApplicantEntity SetMonthlyIncome(decimal monthlyIncome)
    {
        MonthlyIncome = monthlyIncome;
        return this;
    }

    public ApplicantEntity SetApplications(List<RentalApplicationEntity> applications)
    {
        Applications = applications ?? [];
        return this;
    }

    public ApplicantEntity AddApplication(RentalApplicationEntity application)
    {
        Applications.Add(application);
        return this;
    }

    public ApplicantEntity RemoveApplication(RentalApplicationEntity application)
    {
        Applications.Remove(application);
        return this;
    }
}