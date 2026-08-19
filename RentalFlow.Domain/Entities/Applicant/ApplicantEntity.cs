using RentalFlow.Domain.Entities.RentalApplication;

namespace RentalFlow.Domain.Entities.Applicant;

public sealed class ApplicantEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FullName { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public decimal MonthlyIncome { get; private set; }
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public ApplicantEntity(Guid id, string fullName, string cpf, string email, string phone, decimal monthlyIncome, List<RentalApplicationEntity> applications)
    {
        Id = id;
        FullName = fullName;
        Cpf = cpf;
        Email = email;
        Phone = phone;
        MonthlyIncome = monthlyIncome;
        Applications = applications ?? new List<RentalApplicationEntity>();
    }

    public static ApplicantEntity Empty { get; } = new ApplicantEntity
    {
        Id = Guid.NewGuid(),
        FullName = string.Empty,
        Cpf = string.Empty,
        Email = string.Empty,
        Phone = string.Empty,
        MonthlyIncome = 0.0m,
        Applications = []
    };

    private ApplicantEntity() { }

    public ApplicantEntity SetId(Guid id)
    {
        Id = id;
        return this;
    }

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

    public ApplicantEntity SetPhone(string phone)
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

    public ApplicantBuilder ToBuilder() => new()
    {
        Id = Id,
        FullName = FullName,
        Cpf = Cpf,
        Email = Email,
        Phone = Phone,
        MonthlyIncome = MonthlyIncome,
        Applications = Applications
    };
}