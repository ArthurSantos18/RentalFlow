using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Tests.Fixtures;

public static class TestFixtures
{
    public static ApplicantEntity MakeApplicant(
        string fullName = "John Doe",
        string cpf = "52998224725",
        string email = "john.doe@test.com",
        string? phone = "11999999999",
        decimal monthlyIncome = 5000m,
        Guid? id = null,
        bool isActive = true)
    {
        return new ApplicantEntity(fullName, cpf, email, phone, monthlyIncome)
            .SetId(id ?? Guid.NewGuid())
            .SetIsActive(isActive);
    }

    public static TeamEntity MakeTeam(
        string name = "Team Alpha",
        string description = "Default team description",
        Guid? id = null,
        bool isActive = true)
    {
        return new TeamEntity(name, description)
            .SetIsActive(isActive);
    }

    public static PropertyEntity MakeProperty(
        Address? address = null,
        decimal rentPrice = 1500m,
        int bedrooms = 2,
        bool isAvailable = true,
        Guid? id = null,
        bool isActive = true)
    {
        return new PropertyEntity(
            address ?? MakeAddress(),
            rentPrice,
            bedrooms,
            isAvailable)
            .SetId(id ?? Guid.NewGuid())
            .SetIsActive(isActive);
    }

    public static Address MakeAddress(
        string street = "Main Street",
        string number = "100",
        string complement = "Complement",
        string neighborhood = "Downtown",
        string city = "São Paulo",
        string state = "SP",
        string zipCode = "01001000")
    {
        return new Address(street, number, complement, neighborhood, city, state, zipCode);
    }

    public static OperatorEntity MakeOperator(
        string name = "Operator Alpha",
        OperatorRole role = OperatorRole.Broker,
        TeamEntity? team = null,
        Guid? id = null,
        bool isActive = true)
    {
        return new OperatorEntity(name, role, team ?? MakeTeam())
            .SetId(id ?? Guid.NewGuid())
            .SetIsActive(isActive);
    }

    public static RentalApplicationEntity MakeRentalApplication(
        int installments = 12,
        decimal financedAmount = 50000m,
        decimal totalAmount = 60000m,
        DateTime? contractDate = null,
        string proposalNumber = "PRO-TEST01",
        ApplicantEntity? applicant = null,
        PropertyEntity? property = null,
        OperatorEntity? @operator = null,
        Guid? id = null,
        bool isActive = true,
        RentalStatus status = RentalStatus.Draft)
    {
        return new RentalApplicationEntity(
            installments,
            financedAmount,
            totalAmount,
            contractDate ?? DateTime.UtcNow,
            proposalNumber,
            applicant ?? MakeApplicant(),
            property ?? MakeProperty(),
            @operator ?? MakeOperator())
            .SetId(id ?? Guid.NewGuid())
            .SetIsActive(isActive)
            .SetStatus(status);
    }
}