using Azure;
using System.Net.Mail;

namespace RentalFlow.Tests.Fixtures;

public sealed class TestsFixtures(Fixture fixture)
{
    public ApplicantEntity MakeApplicant(
        string? fullName = null,
        string? cpf = null,
        string? email = null,
        string? phone = null,
        decimal? monthlyIncome = null,
        Guid? id = null,
        bool? isActive = null)
    {
        return new ApplicantEntity(
            fullName ?? fixture.Create<string>(),
            cpf ?? GenerateCpf(),
            email ?? fixture.Create<MailAddress>().Address,
            phone ?? fixture.Create<string>(),
            monthlyIncome ?? fixture.Create<decimal>())
            .SetId(id ?? fixture.Create<Guid>())
            .SetIsActive(isActive ?? fixture.Create<bool>());
    }

    public TeamEntity MakeTeam(
        string? name = null,
        string? description = null,
        Guid? id = null,
        bool? isActive = null)
    {
        return new TeamEntity(
            name ?? fixture.Create<string>(),
            description ?? fixture.Create<string>())
            .SetId(id ?? fixture.Create<Guid>())
            .SetIsActive(isActive ?? fixture.Create<bool>());
    }

    public PropertyEntity MakeProperty(
        Address? address = null,
        decimal? rentPrice = null,
        int? bedrooms = null,
        bool? isAvailable = null,
        Guid? id = null,
        bool? isActive = null)
    {
        return new PropertyEntity(
            address ?? MakeAddress(),
            rentPrice ?? fixture.Create<decimal>(),
            bedrooms ?? fixture.Create<int>(),
            isAvailable ?? fixture.Create<bool>())
            .SetId(id ?? fixture.Create<Guid>())
            .SetIsActive(isActive ?? fixture.Create<bool>());
    }

    public Address MakeAddress(
        string? street = null,
        string? number = null,
        string? complement = null,
        string? neighborhood = null,
        string? city = null,
        string? state = null,
        string? zipCode = null)
    {
        return new Address(
            street ?? fixture.Create<string>(),
            number ?? fixture.Create<string>(),
            complement ?? fixture.Create<string>(),
            neighborhood ?? fixture.Create<string>(),
            city ?? fixture.Create<string>(),
            state ?? fixture.Create<string>(),
            zipCode ?? fixture.Create<string>());
    }

    public OperatorEntity MakeOperator(
        string? name = null,
        OperatorRole? role = null,
        TeamEntity? team = null,
        Guid? teamId = null,
        Guid? id = null,
        bool? isActive = null,
        UserEntity? user = null)
    {
        var @operator = new OperatorEntity(
            name ?? fixture.Create<string>(),
            role ?? fixture.Create<OperatorRole>(),
            team ?? MakeTeam(id: teamId))
            .SetId(id ?? fixture.Create<Guid>())
            .SetIsActive(isActive ?? fixture.Create<bool>());

        return @operator.SetUser(user ?? MakeUser(@operator: @operator));
    }

    public RentalApplicationEntity MakeRentalApplication(
        int? installments = null,
        decimal? financedAmount = null,
        decimal? totalAmount = null,
        DateTime? contractDate = null,
        string? proposalNumber = null,
        ApplicantEntity? applicant = null,
        PropertyEntity? property = null,
        OperatorEntity? @operator = null,
        Guid? id = null,
        bool? isActive = null,
        RentalStatus? status = null)
    {
        return new RentalApplicationEntity(
            installments ?? fixture.Create<int>(),
            financedAmount ?? fixture.Create<decimal>(),
            totalAmount ?? fixture.Create<decimal>(),
            contractDate ?? fixture.Create<DateTime>(),
            proposalNumber ?? fixture.Create<string>(),
            applicant ?? MakeApplicant(),
            property ?? MakeProperty(),
            @operator ?? MakeOperator())
            .SetId(id ?? fixture.Create<Guid>())
            .SetIsActive(isActive ?? fixture.Create<bool>())
            .SetStatus(status ?? fixture.Create<RentalStatus>());
    }

    public UserEntity MakeUser(
        string? email = null,
        string? passwordHash = null,
        bool? mustChangePassword = null,
        OperatorEntity? @operator = null,
        Guid? id = null,
        bool? isActive = null)
    {
        return new UserEntity(
            email ?? fixture.Create<MailAddress>().Address,
            passwordHash ?? fixture.Create<string>(),
            mustChangePassword ?? fixture.Create<bool>(),
            @operator ?? MakeOperator(isActive: true))
            .SetId(id ?? fixture.Create<Guid>())
            .SetIsActive(isActive ?? fixture.Create<bool>());
    }

    public UserTokenEntity MakeUserToken(
        UserEntity? user = null,
        string? refreshToken = null,
        DateTime? expiresAt = null,
        DateTime? createdAt = null,
        DateTime? revokedAt = null,
        Guid? id = null)
    {
        return new UserTokenEntity(
            user ?? MakeUser(),
            refreshToken ?? fixture.Create<string>(),
            expiresAt ?? fixture.Create<DateTime>(),
            createdAt ?? fixture.Create<DateTime>(),
            revokedAt)
            .SetId(id ?? fixture.Create<Guid>());
    }

    private static string GenerateCpf()
    {
        return "52998224725";
    }
}