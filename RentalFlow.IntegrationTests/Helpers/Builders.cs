namespace RentalFlow.IntegrationTests.Helpers;

public static class Builders
{
    public static AddApplicantRequest ApplicantRequest(string? cpf = null, string? email = null, string? fullName = null, decimal monthlyIncome = 5000m)
    {
        return new AddApplicantRequest
        {
            FullName = fullName ?? "Test Applicant",
            Cpf = cpf ?? TestData.GenerateCpf(),
            Email = email ?? TestData.GenerateEmail("applicant"),
            Phone = TestData.GeneratePhone(),
            MonthlyIncome = monthlyIncome
        };
    }

    public static AddPropertyRequest PropertyRequest(Address? address = null, decimal rentPrice = 2500m, int bedrooms = 2, bool isAvailable = true)
    {
        return new AddPropertyRequest
        {
            Address = address ?? TestData.GenerateAddress(),
            RentPrice = rentPrice,
            Bedrooms = bedrooms,
            IsAvailable = isAvailable
        };
    }

    public static AddTeamRequest TeamRequest(string? name = null, string? description = null)
    {
        return new AddTeamRequest
        {
            Name = name ?? $"Team-{Guid.NewGuid():N}"[..20],
            Description = description ?? "Test Team Description"
        };
    }

    public static AddOperatorRequest OperatorRequest(Guid teamId, string? name = null, string? email = null, OperatorRole role = OperatorRole.Broker)
    {
        return new AddOperatorRequest
        {
            Name = name ?? "Test Operator",
            Email = email ?? TestData.GenerateEmail("operator"),
            Role = role,
            TeamId = teamId
        };
    }

    public static AddRentalApplicationRequest RentalApplicationRequest(
        Guid applicantId,
        Guid propertyId,
        Guid? operatorId = null,
        decimal financedAmount = 50000m,
        decimal totalAmount = 60000m,
        int installments = 12,
        DateTime? contractDate = null)
    {
        return new AddRentalApplicationRequest
        {
            ApplicantId = applicantId,
            PropertyId = propertyId,
            OperatorId = operatorId,
            FinancedAmount = financedAmount,
            TotalAmount = totalAmount,
            Installments = installments,
            ContractDate = contractDate
        };
    }
}